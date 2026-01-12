import { AfterViewInit, Component, computed, ElementRef, isSignal, NgZone, OnDestroy, OnInit, Renderer2, signal, Signal, viewChild } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { MatTooltip } from '@angular/material/tooltip';
import { GoogleIcon } from "@app/core/components/google-icon/google-icon";
import { PasteWindowOpacityService } from '@app/core/services/paste-window-opacity.service';
import { PasteWindowSizingService } from '@app/core/services/paste-window-sizing.service';
import { PluginsService } from '@app/core/services/plugins.service';
import { ThemeService } from '@app/core/services/theme.service';
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
import { map, Subscription } from 'rxjs';
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
    private readonly pasteWindowSizingService: PasteWindowSizingService,
    private readonly themeService: ThemeService,
    private readonly pasteWindowOpacityService: PasteWindowOpacityService,
  ) {
    this.isDarkMode = toSignal(this.themeService.isDarkTheme$, { requireSync: true });
    this.pinnedClipsHeightPercentage = toSignal(this.pasteWindowSizingService.pinnedClipsHeightPercentage$, { requireSync: true });
    this.opacity = toSignal(this.pasteWindowOpacityService.opacityPercentage$.pipe(map(o => o / 100)), { requireSync: true });
  }


  private readonly subscriptions = new Subscription();
  private readonly searchInputElement = viewChild.required<ElementRef<HTMLInputElement>>('searchInput');
  private readonly splitter = viewChild.required<Splitter>('splitter');
  private readonly regularClipsScrollPanel = viewChild.required<ScrollPanel>('regularClipsScrollPanel');

  private isClipsListUpToDate = false;
  private splitterHandleElement?: HTMLElement | null;

  protected isWindowBlocked = false;
  protected readonly isSettingsMode = signal(false);

  protected readonly isDarkMode: Signal<boolean>;
  protected readonly pinnedClipsHeightPercentage: Signal<number>;
  protected readonly opacity: Signal<number>;

  protected readonly splitterGutterSize = computed(() => {
    const pinnedClips = this.pasteWindowClipsService.orderedPinnedClips();
    const isSettingsMode = this.isSettingsMode();
    if (pinnedClips.length > 0) {
      return isSettingsMode ? 2 : 0;
    }
    return 0;
  });
  protected readonly splitterPanelsGap = computed(() => {
    const pinnedClips = this.pasteWindowClipsService.orderedPinnedClips();
    const isSettingsMode = this.isSettingsMode();
    if (pinnedClips.length > 0) {
      return isSettingsMode ? 1 : 2;
    }
    return 0;
  });

  protected get pinnedClips(): Signal<PasteWindowClip[]> {
    return this.pasteWindowClipsService.orderedPinnedClips;
  }

  protected get regularClips(): Signal<PasteWindowClip[]> {
    return this.pasteWindowClipsService.regularClips;
  }


  async ngOnInit() {
    // todo: maybe move all these subscriptions to PasteWindowClipsService
    this.subscriptions.add(
      this.clipboardListener.clipboardUpdated$.subscribe(() => {
        this.isClipsListUpToDate = false;
      })
    );
    this.subscriptions.add(
      this.pluginsService.pluginInstalled$.subscribe(() => {
        this.isClipsListUpToDate = false;
        this.pasteWindowClipsService.loadPinnedClipsAsync();
      })
    );
    this.subscriptions.add(
      this.pluginsService.pluginUninstalled$.subscribe(() => {
        this.isClipsListUpToDate = false;
        this.pasteWindowClipsService.loadPinnedClipsAsync();
      })
    );
    this.subscriptions.add(
      this.pluginsService.pluginSettingsChanged$.subscribe(() => {
        this.isClipsListUpToDate = false;
        this.pasteWindowClipsService.loadPinnedClipsAsync();
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

    await this.pasteWindowClipsService.loadPinnedClipsAsync();
    this.trackPinnedClipsPanelVisibility();
  }

  ngAfterViewInit(): void {
    const scrollPanelElement = this.regularClipsScrollPanel().contentViewChild?.nativeElement as HTMLElement;
    this.subscriptions.add(this.renderer.listen(scrollPanelElement, 'scroll', this.onScroll.bind(this)));

    this.splitterHandleElement = this.splitter().el.nativeElement.querySelector('.p-splitter-gutter-handle');
    if (this.splitterHandleElement) {
      this.splitterHandleElement.tabIndex = -1;
    }
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }


  protected async settingsMode() {
    this.isSettingsMode.update(v => !v);
    if (this.isSettingsMode()) {
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
      await this.pasteWindowSizingService.setPinnedClipsHeightPercentage(this.splitter().panelSizes[0]);
      if (this.splitterHandleElement) {
        this.splitterHandleElement.tabIndex = -1;
      }
    }
  }


  protected onSearchChanged(search: string) {
    this.pasteWindowClipsService.filter(search);
  }

  protected clearSearch() {
    this.searchInputElement().nativeElement.value = '';
    this.pasteWindowClipsService.filter('');
  }


  protected onScroll(e: Event) {
    const target = e.target as HTMLElement;
    if (target && target.offsetHeight + target.scrollTop >= target.scrollHeight) {
      this.pasteWindowClipsService.loadMoreClipsAsync(10);
    }
  }


  protected async onPasteDataRequested(clipId: number) {
    await this.pasteWindowClipsService.pasteClipAsync(clipId);
  }


  protected async onPreviewDataRequested(clipId: number) {
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


  protected async onPinItemRequested(clipId: number) {
    await this.pasteWindowClipsService.pinClipAsync(clipId);
    this.trackPinnedClipsPanelVisibility();
  }


  protected async onUnpinItemRequested(clipId: number) {
    await this.pasteWindowClipsService.unpinClipAsync(clipId);
    this.trackPinnedClipsPanelVisibility();
  }


  protected async onDeleteItemRequested(clipId: number) {
    await this.pasteWindowClipsService.deleteClipAsync(clipId);
  }


  private trackPinnedClipsPanelVisibility() {
    // todo: try to refactor to computed signal which tracks pinnedClips() array change
    const pinnedClipsPanelElement = this.splitter().el.nativeElement.querySelector('.p-splitterpanel');

    if (this.pinnedClips().length > 0) {
      this.renderer.removeAttribute(pinnedClipsPanelElement, 'hidden');
    }
    else if (this.pinnedClips().length <= 0) {
      this.renderer.setAttribute(pinnedClipsPanelElement, 'hidden', 'true');
    }
  }
}
