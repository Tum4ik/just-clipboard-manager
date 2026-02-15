import { Component, effect, ElementRef, input, output, Renderer2, signal, viewChild } from '@angular/core';
import { MatTooltip } from '@angular/material/tooltip';
import { GoogleIcon } from "@app/core/components/google-icon/google-icon";
import { TranslatePipe } from '@ngx-translate/core';
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'jcm-clip-item',
  templateUrl: './clip-item.component.html',
  styleUrl: './clip-item.component.scss',
  imports: [
    ButtonModule,
    GoogleIcon,
    TranslatePipe,
    MatTooltip,
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
  readonly isPinButtonVisible = input<boolean>(true);
  readonly isUnpinButtonVisible = input<boolean>(false);
  readonly isDeleteButtonVisible = input<boolean>(true);

  readonly pasteDataRequested = output<number>();
  readonly previewDataRequested = output<number>();
  readonly pinItemRequested = output<number>();
  readonly unpinItemRequested = output<number>();
  readonly deleteItemRequested = output<number>();

  protected readonly isMouseOver = signal(false);

  protected onMouseenter() {
    this.isMouseOver.set(true);
  }

  protected onMouseleave() {
    this.isMouseOver.set(false);
  }

  protected paste() {
    this.pasteDataRequested.emit(this.clipId());
  }

  protected preview() {
    this.previewDataRequested.emit(this.clipId());
  }

  protected pin() {
    this.pinItemRequested.emit(this.clipId());
  }

  protected unpin() {
    this.unpinItemRequested.emit(this.clipId());
  }

  protected delete() {
    this.deleteItemRequested.emit(this.clipId());
  }
}
