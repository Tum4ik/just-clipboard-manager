import { DOCUMENT } from '@angular/common';
import { AfterViewInit, Component, ComponentRef, ElementRef, Inject, inputBinding, NgZone, OnDestroy, OnInit, outputBinding, Renderer2, viewChild, ViewContainerRef } from '@angular/core';
import { GoogleIcon } from "@core/components/google-icon/google-icon";
import { TranslatePipe } from '@ngx-translate/core';
import { WebviewWindow } from '@tauri-apps/api/webviewWindow';
import { BlockUIModule } from 'primeng/blockui';
import { IconField } from 'primeng/iconfield';
import { InputIcon } from 'primeng/inputicon';
import { InputText } from 'primeng/inputtext';
import { Panel } from 'primeng/panel';
import { ScrollPanel, ScrollPanelModule } from 'primeng/scrollpanel';
import { BehaviorSubject, debounce, distinctUntilChanged, interval, Subscription } from 'rxjs';
import { ClipsRepository } from '../../core/data/repositories/clips.repository';
import { PluginsService } from '../../core/services/plugins.service';
import { ClipItemComponent } from './components/clip-item/clip-item.component';
import { ClipboardListener } from './services/clipboard-listener.service';
import { PasteDataService } from './services/paste-data.service';
import { PasteWindowService } from './services/paste-window.service';

@Component({
  selector: 'jcm-paste-window',
  templateUrl: './paste-window.component.html',
  styleUrl: './paste-window.component.scss',
  imports: [
    InputText,
    Panel,
    ScrollPanelModule,
    TranslatePipe,
    BlockUIModule,
    IconField,
    InputIcon,
    GoogleIcon,
  ]
})
export class PasteWindowComponent implements OnInit, OnDestroy, AfterViewInit {
  constructor(
    private readonly ngZone: NgZone,
    private readonly pluginsService: PluginsService,
    @Inject(DOCUMENT) private readonly document: Document,
    private readonly renderer: Renderer2,
    private readonly pasteDataService: PasteDataService,
    private readonly pasteWindowService: PasteWindowService,
    private readonly clipboardListener: ClipboardListener
  ) { }


  private readonly searchSubject = new BehaviorSubject<{ text: string; shouldDebounce: boolean; }>(
    { text: '', shouldDebounce: true }
  );
  private readonly subscriptions = new Subscription();
  private readonly clipsRepository = new ClipsRepository();
  private readonly searchInputElement = viewChild.required<ElementRef<HTMLInputElement>>('searchInput');
  private readonly scrollPanel = viewChild.required<ScrollPanel>('scrollPanel');
  private readonly clipsContainer = viewChild.required('clipsContainer', { read: ViewContainerRef });

  private isClipsLoading = false;
  private isWindowVisible = false;
  private isClipsListUpToDate = false;

  isWindowBlocked = false;


  async ngOnInit(): Promise<void> {
    this.subscriptions.add(
      this.searchSubject.pipe(
        distinctUntilChanged((prev, curr) => prev.text === curr.text),
        debounce(search => interval(search.shouldDebounce ? 1000 : 0)),
      ).subscribe(search => {
        this.detachAllClips();
        this.loadClipsAsync(0, 15, search.text);
      })
    );
    this.subscriptions.add(
      this.pasteWindowService.visibility$.subscribe(isVisible => {
        this.isWindowVisible = isVisible;
        if (isVisible && !this.isClipsListUpToDate) {
          this.loadClipsAsync(0, 15);
          this.isClipsListUpToDate = true;
        }
        else if (isVisible) {
          this.scrollPanel().scrollTop(0);
          this.searchInputElement().nativeElement.focus();
        }
        else if (!isVisible) {
          this.clearSearch();
        }
      })
    );
    this.subscriptions.add(
      this.pasteWindowService.isBlocked$.subscribe(isBlocked => {
        this.isWindowBlocked = isBlocked;
      })
    );
    this.subscriptions.add(
      this.clipboardListener.clipboardUpdated$.subscribe(() => {
        this.isClipsListUpToDate = false;
        if (!this.isWindowVisible) {
          this.detachAllClips();
        }
      })
    );
    this.subscriptions.add(
      this.pluginsService.pluginInstalled$.subscribe(() => {
        this.isClipsListUpToDate = false;
        this.detachAllClips();
      })
    );
    this.subscriptions.add(
      this.pluginsService.pluginSettingsChanged$.subscribe(() => {
        this.isClipsListUpToDate = false;
        this.detachAllClips();
      })
    );

    this.loadClipsAsync(0, 15);
    this.isClipsListUpToDate = true;
  }

  ngAfterViewInit(): void {
    const scrollPanelElement = this.scrollPanel().contentViewChild?.nativeElement as HTMLElement;
    this.subscriptions.add(this.renderer.listen(scrollPanelElement, 'scroll', this.onScroll.bind(this)));
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
    this.clipsRepository.disposeAsync();
  }


  onSearchChanged(search: string) {
    this.searchSubject.next({ text: search, shouldDebounce: true });
  }

  clearSearch() {
    this.searchInputElement().nativeElement.value = '';
    this.searchSubject.next({ text: '', shouldDebounce: false });
  }


  onScroll(e: Event) {
    if (!this.isWindowVisible) {
      return;
    }
    const target = e.target as HTMLElement;
    if (target && target.offsetHeight + target.scrollTop >= target.scrollHeight) {
      this.loadClipsAsync(this.clipsContainer().length, 10, this.searchSubject.value.text);
    }
  }


  private loadedClips = new Map<number, ComponentRef<ClipItemComponent>>();

  private async loadClipsAsync(skip: number, take: number, search?: string) {
    if (this.isClipsLoading) {
      return;
    }

    this.isClipsLoading = true;

    const enabledPluginIds = this.pluginsService.enabledPlugins.map(ep => ep.id);
    const clips = await this.clipsRepository.getClipsAsync(enabledPluginIds, skip, take, search);
    if (clips.length > 0) {
      for (const clip of clips) {
        if (this.loadedClips.has(clip.id!)) {
          const component = this.loadedClips.get(clip.id!)!;
          this.clipsContainer().insert(component.hostView);
        }
        else {
          const { plugin } = this.pluginsService.getPlugin(clip.pluginId) ?? {};
          if (!plugin) {
            continue;
          }
          const item = plugin.getRepresentationDataElement(
            { data: clip.representationData, metadata: clip.representationMetadata },
            clip.representationFormatName, this.document
          );
          if (item) {
            const clipItem = this.clipsContainer().createComponent(ClipItemComponent, {
              bindings: [
                inputBinding('clipId', () => clip.id),
                inputBinding('htmlElement', () => item),
                outputBinding<number>('pasteDataRequested', clipId => this.onPasteDataRequested(clipId)),
                outputBinding<number>('previewDataRequested', clipId => this.onPreviewDataRequested(clipId)),
              ]
            });
            this.loadedClips.set(clip.id!, clipItem);
          }
        }
      }
    }

    this.isClipsLoading = false;
  }


  private async onPasteDataRequested(clipId: number) {
    await this.pasteDataService.pasteDataAsync(clipId);
    await this.clipsRepository.updateClippedAtAsync(clipId, new Date());
    this.clipsContainer().move(this.loadedClips.get(clipId)!.hostView, 0);
  }


  private async onPreviewDataRequested(clipId: number) {
    this.pasteWindowService.block();
    const appWindow = new WebviewWindow('clip-preview-window', {
      decorations: false,
      skipTaskbar: true,
      alwaysOnTop: true,
      url: `full-data-preview/${clipId}`
    });
    await appWindow.onCloseRequested(e => {
      this.ngZone.run(() => {
        this.pasteWindowService.unblock();
        this.pasteWindowService.focus();
      });
    });
  }


  private detachAllClips(): void {
    while (this.clipsContainer().length > 0) {
      this.clipsContainer().detach();
    }
  }
}
