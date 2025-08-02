import { Component, input } from '@angular/core';
import { GoogleIcon } from '@app/core/components/google-icon/google-icon';
import { ShadedCardComponent } from "../../../../../shaded-card/shaded-card.component";

@Component({
  selector: 'jcm-plugin-pipeline-card',
  templateUrl: './plugin-pipeline-card.component.html',
  styleUrl: './plugin-pipeline-card.component.scss',
  imports: [
    ShadedCardComponent,
    GoogleIcon,
  ]
})
export class PluginPipelineCardComponent {
  readonly title = input.required<string>();
}
