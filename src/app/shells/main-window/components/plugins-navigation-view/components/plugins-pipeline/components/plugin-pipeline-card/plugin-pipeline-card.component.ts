import { Component, input } from '@angular/core';
import { ShadedCardComponent } from "../../../../../shaded-card/shaded-card.component";

@Component({
  selector: 'jcm-plugin-pipeline-card',
  templateUrl: './plugin-pipeline-card.component.html',
  styleUrl: './plugin-pipeline-card.component.scss',
  imports: [
    ShadedCardComponent
  ]
})
export class PluginPipelineCardComponent {
  readonly title = input.required<string>();
}
