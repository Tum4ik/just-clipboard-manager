import { CdkDrag, CdkDragDrop, CdkDropList } from '@angular/cdk/drag-drop';
import { Component, Signal } from '@angular/core';
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
  constructor(
    private readonly pluginsService: PluginsService,
  ) { }

  get plugins(): Signal<readonly PluginWithAdditionalInfo[]> {
    return this.pluginsService.installedPlugins;
  }

  async pluginsPipelineChanged(e: CdkDragDrop<PluginWithAdditionalInfo[]>) {
    await this.pluginsService.changePluginsOrderAsync(e.previousIndex, e.currentIndex);
  }
}
