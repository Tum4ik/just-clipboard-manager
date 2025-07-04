import { Component, DOCUMENT, ElementRef, Inject, input, NgZone, OnDestroy, OnInit, Renderer2, viewChild } from '@angular/core';
import { TitleBarComponent } from '@core/components/title-bar/title-bar.component';
import { Panel } from 'primeng/panel';
import { ScrollPanelModule } from 'primeng/scrollpanel';
import { ClipsRepository } from '../../../../core/data/repositories/clips.repository';
import { PluginsService } from '../../../../core/services/plugins.service';

@Component({
  selector: 'jcm-clip-full-data-preview',
  templateUrl: './clip-full-data-preview.html',
  styleUrl: './clip-full-data-preview.scss',
  imports: [
    TitleBarComponent,
    Panel,
    ScrollPanelModule,
  ],
})
export class ClipFullDataPreview implements OnInit, OnDestroy {
  constructor(
    private readonly pluginsService: PluginsService,
    private readonly renderer: Renderer2,
    @Inject(DOCUMENT) private readonly document: Document,
    private readonly ngZone: NgZone,
  ) { }

  private readonly clipsRepository = new ClipsRepository();
  private resizeObserver?: ResizeObserver;


  readonly panelWrapper = viewChild.required<ElementRef<HTMLElement>>('panelWrapper');
  readonly contentContainer = viewChild.required<ElementRef<HTMLElement>>('previewContent');

  readonly clipId = input.required<number>();

  scrollHeight = '0';

  ngOnInit(): void {
    const panelWrapperElement = this.panelWrapper().nativeElement;
    this.resizeObserver = new ResizeObserver(entries => {
      for (const entry of entries) {
        if (entry.target === panelWrapperElement) {
          this.ngZone.run(() => {
            this.scrollHeight = `${panelWrapperElement.clientHeight - 6 - 2 * 4}px`;
          });
        }
      }
    });
    this.resizeObserver.observe(panelWrapperElement);
    this.loadDataPreviewElement();
  }

  ngOnDestroy(): void {
    this.clipsRepository.disposeAsync();
    this.resizeObserver?.disconnect();
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
    this.renderer.appendChild(this.contentContainer().nativeElement, dataPreviewElement);
  }
}
