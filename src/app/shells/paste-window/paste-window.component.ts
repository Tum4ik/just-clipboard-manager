import { AfterViewInit, Component, ElementRef, NgZone, OnDestroy, OnInit, Renderer2, Signal, TemplateRef, viewChild } from '@angular/core';
import { MatTooltip } from '@angular/material/tooltip';
import { GoogleIcon } from "@app/core/components/google-icon/google-icon";
import { PluginsService } from '@app/core/services/plugins.service';
import { SettingsService } from '@app/core/services/settings.service';
import { TranslatePipe } from '@ngx-translate/core';
import { WebviewWindow } from '@tauri-apps/api/webviewWindow';
import { BlockUI } from 'primeng/blockui';
import { Button } from "primeng/button";
import { IconField } from 'primeng/iconfield';
import { InputIcon } from 'primeng/inputicon';
import { InputText } from 'primeng/inputtext';
import { Panel } from 'primeng/panel';
import { ScrollPanel } from 'primeng/scrollpanel';
import { Splitter } from 'primeng/splitter';
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
    ScrollPanel,
    TranslatePipe,
    BlockUI,
    IconField,
    InputIcon,
    GoogleIcon,
    ClipItemComponent,
    Button,
    Splitter,
    MatTooltip,
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
    private readonly settingsService: SettingsService,
  ) { }


  private readonly subscriptions = new Subscription();
  private readonly clipsRepository = new ClipsRepository();
  private readonly searchInputElement = viewChild.required<ElementRef<HTMLInputElement>>('searchInput');
  private readonly splitter = viewChild.required<Splitter>('splitter');
  private readonly regularClipsScrollPanel = viewChild.required<ScrollPanel>('regularClipsScrollPanel');

  private isClipsListUpToDate = false;
  private pinnedClipsPanelTemplate?: TemplateRef<HTMLElement>;
  private splitterHandleElement?: HTMLElement | null;

  isWindowBlocked = false;
  isSettingsMode = false;
  splitterPanelSizes: number[] = [];

  get pinnedClips(): Signal<PasteWindowClip[]> {
    return this.pasteWindowClipsService.orderedPinnedClips;
  }

  get regularClips(): Signal<PasteWindowClip[]> {
    return this.pasteWindowClipsService.regularClips;
  }


  ngOnInit() {
    // todo: maybe move all these subscriptions to PasteWindowClipsService
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

    this.settingsService.getPasteWindowPanelSizesAsync().then(sizes => this.splitterPanelSizes = sizes);

    this.pasteWindowClipsService.loadPinnedClipsAsync();
  }

  ngAfterViewInit(): void {
    const scrollPanelElement = this.regularClipsScrollPanel().contentViewChild?.nativeElement as HTMLElement;
    this.subscriptions.add(this.renderer.listen(scrollPanelElement, 'scroll', this.onScroll.bind(this)));

    this.pinnedClipsPanelTemplate = this.splitter().panels[0];
    this.splitterHandleElement = this.splitter().el.nativeElement.querySelector('.p-splitter-gutter-handle');
    if (this.splitterHandleElement) {
      this.splitterHandleElement.tabIndex = -1;
    }
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
      if (this.splitterHandleElement) {
        this.splitterHandleElement.tabIndex = 0;
      }
    }
    else {
      this.pasteWindowService.allowHide();
      await this.pasteWindowService.disableResizeAsync();
      await this.pasteWindowService.rememberWindowSizeAsync();
      await this.settingsService.setPasteWindowPanelSizesAsync(this.splitter().panelSizes);
      if (this.splitterHandleElement) {
        this.splitterHandleElement.tabIndex = -1;
      }
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
    this.trackPinnedClipsPanelVisibility();
  }


  async onUnpinItemRequested(clipId: number) {
    await this.pasteWindowClipsService.unpinClipAsync(clipId);
    this.trackPinnedClipsPanelVisibility();
  }


  async onDeleteItemRequested(clipId: number) {
    await this.pasteWindowClipsService.deleteClipAsync(clipId);
  }


  private trackPinnedClipsPanelVisibility() {
    if (this.splitter().panels.length === 1 && this.pinnedClips().length > 0 && this.pinnedClipsPanelTemplate) {
      this.splitter().panels.unshift(this.pinnedClipsPanelTemplate);
    }
    else if (this.pinnedClips().length <= 0 && this.splitter().panels.length === 2) {
      this.splitter().panels.splice(0, 1);
    }
  }
}
