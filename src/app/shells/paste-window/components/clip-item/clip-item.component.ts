import { Component, effect, ElementRef, input, Renderer2, viewChild } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { ClipsRepository } from '../../../../core/data/repositories/clips.repository';
import { PasteDataService } from '../../services/paste-data.service';

@Component({
  selector: 'jcm-clip-item',
  templateUrl: './clip-item.component.html',
  styleUrl: './clip-item.component.scss',
  imports: [
    ButtonModule
  ],
})
export class ClipItemComponent {
  constructor(
    private readonly renderer: Renderer2,
    private readonly pasteDataService: PasteDataService,
  ) {
    effect(() => {
      this.renderer.appendChild(this.button().nativeElement, this.htmlElement());
    });
  }

  private readonly button = viewChild.required<ElementRef>('buttonContent');

  readonly clipId = input.required<number>();
  readonly formatId = input.required<number>();
  readonly htmlElement = input.required<HTMLElement>();

  async paste() {
    const clipsRepository = new ClipsRepository();
    const data = await clipsRepository.getClipDataAsync(this.clipId());
    if (data) {
      await this.pasteDataService.pasteDataAsync(data, this.formatId());
    }


  }
}
