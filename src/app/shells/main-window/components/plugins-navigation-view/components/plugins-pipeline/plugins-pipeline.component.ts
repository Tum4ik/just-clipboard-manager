import { CdkDrag, CdkDragDrop, CdkDropList, moveItemInArray } from '@angular/cdk/drag-drop';
import { Component, OnInit } from '@angular/core';
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
    ScrollViewComponent
  ]
})
export class PluginsPipelineComponent implements OnInit {
  constructor(
    private readonly pluginsService: PluginsService,
  ) { }

  plugins: PluginWithAdditionalInfo[] = [];

  ngOnInit(): void {
    this.plugins = [...this.pluginsService.installedPlugins];
  }

  async pluginsPipelineChanged(e: CdkDragDrop<PluginWithAdditionalInfo[]>) {
    moveItemInArray(this.plugins, e.previousIndex, e.currentIndex);
    await this.pluginsService.changePluginsOrderAsync(e.previousIndex, e.currentIndex);
  }
}
