import { AsyncPipe } from '@angular/common';
import { Component, ElementRef, NgZone, OnDestroy, OnInit, viewChild } from '@angular/core';
import { InputTextModule } from 'primeng/inputtext';
import { Panel } from 'primeng/panel';
import { Scroller } from 'primeng/scroller';
import { of } from 'rxjs';
import { ClipsService } from '../core/services/clips.service';

@Component({
  selector: 'jcm-paste-window',
  templateUrl: './paste-window.component.html',
  styleUrl: './paste-window.component.scss',
  imports: [
    InputTextModule,
    Scroller,
    Panel,
    AsyncPipe
  ]
})
export class PasteWindowComponent implements OnInit, OnDestroy {
  constructor(
    private readonly ngZone: NgZone,
    private readonly clipsService: ClipsService
  ) { }

  private resizeObserver?: ResizeObserver;
  private readonly rootElement = viewChild.required<ElementRef<HTMLElement>>('root');
  private readonly searchInputElement = viewChild.required<ElementRef<HTMLInputElement>>('searchInput');

  readonly scrollableAreaMarginTop = 4;
  readonly scrollableAreaMarginBottom = 4;

  scrollableAreaHeight = '0px';

  private _items: HTMLElement[] = [];
  items = of(this._items);

  ngOnInit(): void {
    this.resizeObserver = new ResizeObserver(entries => {
      this.ngZone.run(() => {
        for (const entry of entries) {
          if (entry.target === this.rootElement().nativeElement) {
            const windowHeight = this.rootElement().nativeElement.clientHeight;
            const searchInputHeight = this.searchInputElement().nativeElement.clientHeight;
            const topBottomMargins = this.scrollableAreaMarginTop + this.scrollableAreaMarginBottom;
            this.scrollableAreaHeight = `${windowHeight - searchInputHeight - topBottomMargins}px`;
          }
        }
      });
    });
    this.resizeObserver.observe(this.rootElement().nativeElement);
  }

  ngOnDestroy(): void {
    this.resizeObserver?.disconnect();
  }

  trackByFn(index: number, item: string): number {
    return index;
  }

  onLazyLoad(e: { first: number, last: number; }): void {
    this.clipsService.getClipsAsync({
      skip: 0,
      take: 1,
    }).then(clips => {
      this._items.push(...clips);
    });
  }
}
