import { AsyncPipe, DOCUMENT } from '@angular/common';
import { Component, ElementRef, Inject, NgZone, OnDestroy, OnInit, viewChild } from '@angular/core';
import { InputText } from 'primeng/inputtext';
import { Panel } from 'primeng/panel';
import { Scroller, ScrollerScrollEvent } from 'primeng/scroller';
import { BehaviorSubject } from 'rxjs';
import { ClipsRepository } from '../../core/data/repositories/clips.repository';
import { PluginsService } from '../../core/services/plugins.service';
import { ClipItemComponent } from './components/clip-item/clip-item.component';
import { TranslatePipe } from '@ngx-translate/core';

@Component({
  selector: 'jcm-paste-window',
  templateUrl: './paste-window.component.html',
  styleUrl: './paste-window.component.scss',
  imports: [
    InputText,
    Scroller,
    Panel,
    AsyncPipe,
    TranslatePipe,
    ClipItemComponent,
  ]
})
export class PasteWindowComponent implements OnInit, OnDestroy {
  constructor(
    private readonly ngZone: NgZone,
    private readonly pluginsService: PluginsService,
    @Inject(DOCUMENT) private readonly document: Document
  ) { }

  private resizeObserver?: ResizeObserver;
  private readonly clipsRepository = new ClipsRepository();
  private readonly rootElement = viewChild.required<ElementRef<HTMLElement>>('root');
  private readonly searchInputElement = viewChild.required<ElementRef<HTMLInputElement>>('searchInput');
  private readonly scroller = viewChild.required<Scroller>(Scroller);

  private loadedClipsCount = 0;
  private isClipsLoading = false;

  readonly SCROLLABLE_AREA_MARGIN_TOP = 4;
  readonly SCROLLABLE_AREA_MARGIN_BOTTOM = 4;

  scrollableAreaHeight = '0px';
  // scrollableAreaWidth = 0;

  private _items = new BehaviorSubject<HTMLElement[]>([]);
  items = this._items.asObservable();


  ngOnInit(): void {
    this.resizeObserver = new ResizeObserver(entries => {
      this.ngZone.run(() => {
        for (const entry of entries) {
          if (entry.target === this.rootElement().nativeElement) {
            const windowHeight = this.rootElement().nativeElement.clientHeight;
            const searchInputHeight = this.searchInputElement().nativeElement.clientHeight;
            const topBottomMargins = this.SCROLLABLE_AREA_MARGIN_TOP + this.SCROLLABLE_AREA_MARGIN_BOTTOM;
            this.scrollableAreaHeight = `${windowHeight - searchInputHeight - topBottomMargins}px`;

            // const scrollerElement = this.scroller().el.nativeElement as HTMLElement;
            // this.scrollableAreaWidth = scrollerElement.getElementsByClassName('p-virtualscroller')[0].clientWidth;
          }
        }
      });
    });
    this.resizeObserver.observe(this.rootElement().nativeElement);

    this.loadClipsAsync(0, 15);
  }

  ngOnDestroy(): void {
    this.resizeObserver?.disconnect();
    this.clipsRepository.disposeAsync();
  }

  trackByFn(index: number, item: string): number {
    return index;
  }


  onScroll(e: ScrollerScrollEvent) {
    const target = e.originalEvent?.target as HTMLElement;
    if (target && target.offsetHeight + target.scrollTop >= target.scrollHeight) {
      this.loadClipsAsync(this.loadedClipsCount, 10);
    }
  }


  private async loadClipsAsync(skip: number, take: number) {
    if (this.isClipsLoading) {
      return;
    }

    this.isClipsLoading = true;

    const clips = await this.clipsRepository.getClipsAsync(skip, take);
    this.loadedClipsCount += clips.length;
    for (const clip of clips) {
      const plugin = this.pluginsService.getPlugin(clip.pluginId);
      const item = plugin?.getRepresentationDataElement(clip.representationData, clip.format, this.document);
      if (item) {
        this._items.next([...this._items.value, item]);
      }
    }

    this.isClipsLoading = false;
  }
}
