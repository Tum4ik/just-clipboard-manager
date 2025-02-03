import { AsyncPipe } from '@angular/common';
import { Component, ElementRef, NgZone, OnDestroy, OnInit, viewChild } from '@angular/core';
import { InputText } from 'primeng/inputtext';
import { Panel } from 'primeng/panel';
import { Scroller } from 'primeng/scroller';
import { of } from 'rxjs';

@Component({
  selector: 'jcm-paste-window',
  templateUrl: './paste-window.component.html',
  styleUrl: './paste-window.component.scss',
  imports: [
    InputText,
    Scroller,
    Panel,
    AsyncPipe
  ]
})
export class PasteWindowComponent implements OnInit, OnDestroy {
  constructor(
    private readonly ngZone: NgZone
  ) { }

  private resizeObserver?: ResizeObserver;
  private readonly rootElement = viewChild.required<ElementRef<HTMLElement>>('root');
  private readonly searchInputElement = viewChild.required<ElementRef<HTMLInputElement>>('searchInput');

  readonly SCROLLABLE_AREA_MARGIN_TOP = 4;
  readonly SCROLLABLE_AREA_MARGIN_BOTTOM = 4;

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
            const topBottomMargins = this.SCROLLABLE_AREA_MARGIN_TOP + this.SCROLLABLE_AREA_MARGIN_BOTTOM;
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

  }
}
