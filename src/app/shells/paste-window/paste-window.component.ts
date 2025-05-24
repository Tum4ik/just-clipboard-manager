import { DOCUMENT } from '@angular/common';
import { AfterViewInit, Component, ComponentRef, ElementRef, Inject, NgZone, OnDestroy, OnInit, Renderer2, viewChild, ViewContainerRef } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';
import { InputText } from 'primeng/inputtext';
import { Panel } from 'primeng/panel';
import { ScrollPanel, ScrollPanelModule } from 'primeng/scrollpanel';
import { BehaviorSubject, debounceTime, distinctUntilChanged, Subscription } from 'rxjs';
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

  private resizeObserver?: ResizeObserver;
  private searchSubject = new BehaviorSubject<string>('');
  private subscriptions = new Subscription();
  private readonly clipsRepository = new ClipsRepository();
  private readonly rootElement = viewChild.required<ElementRef<HTMLElement>>('root');
  private readonly searchInputElement = viewChild.required<ElementRef<HTMLInputElement>>('searchInput');
  private readonly scrollPanel = viewChild.required<ScrollPanel>('scrollPanel');
  private readonly clipsContainer = viewChild.required<unknown, ViewContainerRef>('clipsContainer', { read: ViewContainerRef });

  private isClipsLoading = false;
  private isWindowVisible = false;
  private isClipsListUpToDate = false;

  readonly SCROLLABLE_AREA_MARGIN_TOP = 4;
  readonly SCROLLABLE_AREA_MARGIN_BOTTOM = 4;

  scrollableAreaHeight = '0px';


  async ngOnInit(): Promise<void> {
    this.resizeObserver = new ResizeObserver(entries => {
      this.ngZone.run(() => {
        for (const entry of entries) {
          if (entry.target === this.rootElement().nativeElement) {
            const windowHeight = this.rootElement().nativeElement.clientHeight;
            const searchInputHeight = this.searchInputElement().nativeElement.clientHeight;
            const topBottomMargins = this.SCROLLABLE_AREA_MARGIN_TOP + this.SCROLLABLE_AREA_MARGIN_BOTTOM;
            this.scrollableAreaHeight = `${windowHeight - searchInputHeight - 2 * topBottomMargins}px`;
          }
        }
      });
    });
    this.resizeObserver.observe(this.rootElement().nativeElement);

    this.subscriptions.add(
      this.searchSubject.pipe(
        debounceTime(1000),
        distinctUntilChanged(),
      ).subscribe(search => {
        this.detachAllClips();
        this.loadClipsAsync(0, 15, search);
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
          this.searchSubject.next('');
          this.searchInputElement().nativeElement.value = '';
        }
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
    const scrollPanelElement = this.scrollPanel().containerViewChild?.nativeElement as HTMLElement;
    const scrollPanelContent = scrollPanelElement.querySelector('.p-scrollpanel-content');
    this.subscriptions.add(this.renderer.listen(scrollPanelContent, 'scroll', this.onScroll.bind(this)));
  }

  ngOnDestroy(): void {
    this.resizeObserver?.disconnect();
    this.subscriptions.unsubscribe();
    this.clipsRepository.disposeAsync();
  }


  onSearchChanged(search: string) {
    this.searchSubject.next(search);
  }


  onScroll(e: Event) {
    if (!this.isWindowVisible) {
      return;
    }
    const target = e.target as HTMLElement;
    if (target && target.offsetHeight + target.scrollTop >= target.scrollHeight) {
      this.loadClipsAsync(this.clipsContainer().length, 10, this.searchSubject.value);
    }
  }


  private loadedClips = new Map<number, { formatId: number, component: ComponentRef<ClipItemComponent>; }>();

  private async loadClipsAsync(skip: number, take: number, search?: string) {
    if (this.isClipsLoading) {
      return;
    }

    this.isClipsLoading = true;

    const enabledPluginIds = this.pluginsService.enabledPlugins.map(ep => ep.id);
    const clips = await this.clipsRepository.getClipPreviewsAsync(enabledPluginIds, skip, take, search);
    if (clips.length > 0) {
      for (const clip of clips) {
        if (this.loadedClips.has(clip.id!)) {
          const component = this.loadedClips.get(clip.id!)!.component;
          this.clipsContainer().insert(component.hostView);
        }
        else {
          const { plugin } = this.pluginsService.getPlugin(clip.pluginId) ?? {};
          if (!plugin) {
            continue;
          }
          const item = plugin.getRepresentationDataElement(
            { data: clip.representationData, metadata: clip.representationMetadata },
            clip.format, this.document
          );
          if (item) {
            const clipItem = this.clipsContainer().createComponent(ClipItemComponent);
            this.loadedClips.set(clip.id!, { formatId: clip.formatId, component: clipItem });
            clipItem.setInput('clipId', clip.id);
            clipItem.setInput('htmlElement', item);
            clipItem.instance.pasteDataRequested.subscribe(this.onPasteDataRequested.bind(this));
          }
        }
      }
    }

    this.isClipsLoading = false;
  }


  private async onPasteDataRequested(clipId: number) {
    const data = await this.clipsRepository.getClipDataAsync(clipId);
    if (data) {
      await this.pasteDataService.pasteDataAsync(data, this.loadedClips.get(clipId)!.formatId);
      await this.clipsRepository.updateClippedAtAsync(clipId, new Date());
      this.clipsContainer().move(this.loadedClips.get(clipId)!.component.hostView, 0);
    }
  }


  private detachAllClips(): void {
    while (this.clipsContainer().length > 0) {
      this.clipsContainer().detach();
    }
  }
}
