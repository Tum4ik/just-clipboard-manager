import { CdkDrag, CdkDragDrop, CdkDropList } from '@angular/cdk/drag-drop';
import { Component, inject } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { GoogleIcon } from "@app/core/components/google-icon/google-icon";
import { TranslatePipe } from '@ngx-translate/core';
import { PluginsService, PluginWithAdditionalInfo } from '../../../../../../core/services/plugins.service';
import { ScrollViewComponent } from "../../../scroll-view/scroll-view.component";
import { PluginPipelineCardComponent } from "./components/plugin-pipeline-card/plugin-pipeline-card.component";

@Component({
  selector: 'jcm-plugins-pipeline',
  templateUrl: './plugins-pipeline.component.html',
  styleUrl: './plugins-pipeline.component.scss',
  imports: [
    CdkDropList,
    CdkDrag,
    PluginPipelineCardComponent,
    TranslatePipe,
    ScrollViewComponent,
    GoogleIcon
  ]
})
export class PluginsPipelineComponent {
  private readonly pluginsService = inject(PluginsService);

  protected readonly plugins = toSignal(this.pluginsService.installedPlugins);

  protected async pluginsPipelineChanged(e: CdkDragDrop<PluginWithAdditionalInfo[]>) {
    await this.pluginsService.changePluginsOrderAsync(e.previousIndex, e.currentIndex);
  }
}
