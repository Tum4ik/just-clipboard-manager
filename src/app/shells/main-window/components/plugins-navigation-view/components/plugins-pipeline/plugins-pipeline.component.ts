import { CdkDrag, CdkDropList } from '@angular/cdk/drag-drop';
import { Component } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';
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
export class PluginsPipelineComponent {

}
