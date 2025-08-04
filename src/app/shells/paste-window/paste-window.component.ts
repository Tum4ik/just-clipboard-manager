import { AfterViewInit, Component, ElementRef, NgZone, OnDestroy, OnInit, Renderer2, Signal, viewChild } from '@angular/core';
import { GoogleIcon } from "@app/core/components/google-icon/google-icon";
import { PluginsService } from '@app/core/services/plugins.service';
import { TranslatePipe } from '@ngx-translate/core';
import { WebviewWindow } from '@tauri-apps/api/webviewWindow';
import { BlockUIModule } from 'primeng/blockui';
import { Button } from "primeng/button";
import { IconField } from 'primeng/iconfield';
import { InputIcon } from 'primeng/inputicon';
import { InputText } from 'primeng/inputtext';
import { Panel } from 'primeng/panel';
import { ScrollPanel, ScrollPanelModule } from 'primeng/scrollpanel';
import { Subscription } from 'rxjs';
import { ClipsRepository } from '../../core/data/repositories/clips.repository';
import { ClipItemComponent } from './components/clip-item/clip-item.component';
import { ClipboardListener } from './services/clipboard-listener.service';
import { PasteWindowClip, PasteWindowClipsService } from './services/paste-window-clips.service';
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
    ClipItemComponent,
    Button
  ]
})
export class PasteWindowComponent implements OnInit, OnDestroy, AfterViewInit {
  constructor(
    private readonly renderer: Renderer2,
    private readonly ngZone: NgZone,
    private readonly pasteWindowService: PasteWindowService,
    private readonly pasteWindowClipsService: PasteWindowClipsService,
    private readonly clipboardListener: ClipboardListener,
    private readonly pluginsService: PluginsService,
  ) { }


  private readonly subscriptions = new Subscription();
  private readonly clipsRepository = new ClipsRepository();
  private readonly searchInputElement = viewChild.required<ElementRef<HTMLInputElement>>('searchInput');
  private readonly regularClipsScrollPanel = viewChild.required<ScrollPanel>('regularClipsScrollPanel');

  private isClipsListUpToDate = false;

  readonly pinnedClipsPanelMaxHeight = 138;

  isWindowBlocked = false;
  isSettingsMode = false;

  get pinnedClips(): Signal<PasteWindowClip[]> {
    return this.pasteWindowClipsService.orderedPinnedClips;
  }

  get regularClips(): Signal<PasteWindowClip[]> {
    return this.pasteWindowClipsService.regularClips;
  }


  ngOnInit() {
    this.subscriptions.add(
      this.clipboardListener.clipboardUpdated$.subscribe(() => {
        this.isClipsListUpToDate = false;
      })
    );
    this.subscriptions.add(
      this.pluginsService.pluginInstalled$.subscribe(() => {
        this.isClipsListUpToDate = false;
      })
    );
    this.subscriptions.add(
      this.pluginsService.pluginSettingsChanged$.subscribe(() => {
        this.isClipsListUpToDate = false;
      })
    );
    this.subscriptions.add(
      this.pasteWindowService.visibility$.subscribe(isVisible => {
        if (isVisible) {
          if (!this.isClipsListUpToDate) {
            this.pasteWindowClipsService.loadClipsFromScratchAsync();
            this.isClipsListUpToDate = true;
          }
          this.regularClipsScrollPanel().scrollTop(0);
          this.searchInputElement().nativeElement.focus();
        }
        else if (!isVisible) {
          this.clearSearch();
        }
      })
    );

    this.pasteWindowClipsService.loadPinnedClipsAsync();
  }

  ngAfterViewInit(): void {
    const scrollPanelElement = this.regularClipsScrollPanel().contentViewChild?.nativeElement as HTMLElement;
    this.subscriptions.add(this.renderer.listen(scrollPanelElement, 'scroll', this.onScroll.bind(this)));
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
    this.clipsRepository.disposeAsync();
  }


  async settingsMode() {
    this.isSettingsMode = !this.isSettingsMode;
    if (this.isSettingsMode) {
      this.pasteWindowService.disallowHide();
      await this.pasteWindowService.enableResizeAsync();
    }
    else {
      this.pasteWindowService.allowHide();
      await this.pasteWindowService.disableResizeAsync();
    }
  }


  onSearchChanged(search: string) {
    this.pasteWindowClipsService.filter(search);
  }

  clearSearch() {
    this.searchInputElement().nativeElement.value = '';
    this.pasteWindowClipsService.filter('');
  }


  onScroll(e: Event) {
    const target = e.target as HTMLElement;
    if (target && target.offsetHeight + target.scrollTop >= target.scrollHeight) {
      this.pasteWindowClipsService.loadMoreClipsAsync(10);
    }
  }


  async onPasteDataRequested(clipId: number) {
    await this.pasteWindowClipsService.pasteClipAsync(clipId);
  }


  async onPreviewDataRequested(clipId: number) {
    this.isWindowBlocked = true;
    this.pasteWindowService.disallowHide();
    const appWindow = new WebviewWindow('clip-preview-window', {
      decorations: false,
      skipTaskbar: true,
      alwaysOnTop: true,
      url: `full-data-preview/${clipId}`
    });
    await appWindow.onCloseRequested(e => {
      this.ngZone.run(() => {
        this.isWindowBlocked = false;
        this.pasteWindowService.allowHide();
        this.pasteWindowService.focus();
      });
    });
  }


  async onPinItemRequested(clipId: number) {
    await this.pasteWindowClipsService.pinClipAsync(clipId);
  }


  async onUnpinItemRequested(clipId: number) {
    await this.pasteWindowClipsService.unpinClipAsync(clipId);
  }


  async onDeleteItemRequested(clipId: number) {
    await this.pasteWindowClipsService.deleteClipAsync(clipId);
  }
}
