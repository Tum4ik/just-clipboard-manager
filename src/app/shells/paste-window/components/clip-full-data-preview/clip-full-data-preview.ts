import { Component, DOCUMENT, ElementRef, Inject, input, OnDestroy, OnInit, Renderer2 } from '@angular/core';
import { TitleBarComponent } from '@core/components/title-bar/title-bar.component';
import { ClipsRepository } from '../../../../core/data/repositories/clips.repository';
import { PluginsService } from '../../../../core/services/plugins.service';

@Component({
  selector: 'jcm-clip-full-data-preview',
  templateUrl: './clip-full-data-preview.html',
  styleUrl: './clip-full-data-preview.scss',
  imports: [
    TitleBarComponent
  ],
})
export class ClipFullDataPreview implements OnInit, OnDestroy {
  constructor(
    private readonly pluginsService: PluginsService,
    private readonly renderer: Renderer2,
    @Inject(DOCUMENT) private readonly document: Document,
    private readonly elementRef: ElementRef,
  ) { }

  private readonly clipsRepository = new ClipsRepository();

  readonly clipId = input.required<number>();

  ngOnInit(): void {
    this.loadDataPreviewElement();
  }

  ngOnDestroy(): void {
    this.clipsRepository.disposeAsync();
  }

  private async loadDataPreviewElement() {
    const result = await this.clipsRepository.getClipFullDataPreviewAsync(this.clipId());
    if (!result) {
      return;
    }

    const { pluginId, data, format } = result;
    const plugin = this.pluginsService.getPlugin(pluginId);
    if (!plugin) {
      return;
    }

    const dataPreviewElement = plugin.plugin.getFullDataPreviewElement(data, format, this.document);
    this.renderer.appendChild(this.elementRef.nativeElement, dataPreviewElement);
  }
}
