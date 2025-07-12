import { Component, effect, ElementRef, input, output, Renderer2, signal, viewChild } from '@angular/core';
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'jcm-clip-item',
  templateUrl: './clip-item.component.html',
  styleUrl: './clip-item.component.scss',
  imports: [
    ButtonModule
  ],
  host: {
    '(mouseenter)': 'onMouseenter()',
    '(mouseleave)': 'onMouseleave()',
  }
})
export class ClipItemComponent {
  constructor(
    private readonly renderer: Renderer2,
  ) {
    effect(() => {
      this.renderer.appendChild(this.button().nativeElement, this.htmlElement());
    });
  }

  private readonly button = viewChild.required<ElementRef>('buttonContent');

  readonly clipId = input.required<number>();
  readonly htmlElement = input.required<HTMLElement>();

  readonly pasteDataRequested = output<number>();
  readonly previewDataRequested = output<number>();
  readonly deleteItemRequested = output<number>();

  readonly isActionButtonsVisible = signal(false);

  onMouseenter() {
    this.isActionButtonsVisible.set(true);
  }

  onMouseleave() {
    this.isActionButtonsVisible.set(false);
  }

  paste() {
    this.pasteDataRequested.emit(this.clipId());
  }

  preview() {
    this.previewDataRequested.emit(this.clipId());
  }

  delete() {
    this.deleteItemRequested.emit(this.clipId());
  }
}
