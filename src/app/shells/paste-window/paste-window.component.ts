import { DOCUMENT } from '@angular/common';
import { AfterViewInit, Component, ElementRef, Inject, NgZone, OnDestroy, OnInit, Renderer2, viewChild, ViewContainerRef } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';
import { InputText } from 'primeng/inputtext';
import { Panel } from 'primeng/panel';
import { ScrollPanel, ScrollPanelModule } from 'primeng/scrollpanel';
import { ClipsRepository } from '../../core/data/repositories/clips.repository';
import { PluginsService } from '../../core/services/plugins.service';
import { ClipItemComponent } from './components/clip-item/clip-item.component';

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
  ) { }

  private resizeObserver?: ResizeObserver;
  private scrollListenUnsubscriber?: () => void;
  private readonly clipsRepository = new ClipsRepository();
  private readonly rootElement = viewChild.required<ElementRef<HTMLElement>>('root');
  private readonly searchInputElement = viewChild.required<ElementRef<HTMLInputElement>>('searchInput');
  private readonly scrollPanel = viewChild.required<ScrollPanel>('scrollPanel');
  private readonly clipsContainer = viewChild.required<unknown, ViewContainerRef>('clipsContainer', { read: ViewContainerRef });

  private loadedClipsCount = 0;
  private isClipsLoading = false;

  readonly SCROLLABLE_AREA_MARGIN_TOP = 4;
  readonly SCROLLABLE_AREA_MARGIN_BOTTOM = 4;

  scrollableAreaHeight = '0px';


  ngOnInit(): void {
    this.resizeObserver = new ResizeObserver(entries => {
      this.ngZone.run(() => {
        for (const entry of entries) {
          if (entry.target === this.rootElement().nativeElement) {
            const windowHeight = this.rootElement().nativeElement.clientHeight;
            const searchInputHeight = this.searchInputElement().nativeElement.clientHeight;
            const topBottomMargins = this.SCROLLABLE_AREA_MARGIN_TOP + this.SCROLLABLE_AREA_MARGIN_BOTTOM;
            this.scrollableAreaHeight = `${windowHeight - searchInputHeight - topBottomMargins}px`;
          }
        }
      });
    });
    this.resizeObserver.observe(this.rootElement().nativeElement);

    this.loadClipsAsync(0, 15);
  }

  ngAfterViewInit(): void {
    const scrollPanelElement = this.scrollPanel().containerViewChild?.nativeElement as HTMLElement;
    const scrollPanelContent = scrollPanelElement.querySelector('.p-scrollpanel-content');
    this.scrollListenUnsubscriber = this.renderer.listen(scrollPanelContent, 'scroll', this.onScroll.bind(this));
  }

  ngOnDestroy(): void {
    this.resizeObserver?.disconnect();
    this.scrollListenUnsubscriber?.();
    this.clipsRepository.disposeAsync();
  }


  onScroll(e: Event) {
    const target = e.target as HTMLElement;
    if (target && target.offsetHeight + target.scrollTop >= target.scrollHeight) {
      this.loadClipsAsync(this.loadedClipsCount, 10);
    }
  }


  private async loadClipsAsync(skip: number, take: number) {
    if (this.isClipsLoading) {
      return;
    }

    this.isClipsLoading = true;

    const clips = await this.clipsRepository.getClipPreviewsAsync(skip, take);
    if (clips.length > 0) {
      this.loadedClipsCount += clips.length;
      for (const clip of clips) {
        const plugin = this.pluginsService.getPlugin(clip.pluginId);
        const item = plugin?.getRepresentationDataElement(clip.representationData, clip.format, this.document);
        if (item) {
          const clipItem = this.clipsContainer().createComponent(ClipItemComponent);
          clipItem.setInput('clipId', clip.id);
          clipItem.setInput('formatId', clip.formatId);
          clipItem.setInput('htmlElement', item);
        }
      }
    }

    this.isClipsLoading = false;
  }
}
