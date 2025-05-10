import { Component, input } from '@angular/core';
import { Menubar } from 'primeng/menubar';
import { ShadedCardComponent } from "../shaded-card/shaded-card.component";

@Component({
  selector: 'jcm-settings-card',
  templateUrl: './settings-card.component.html',
  styleUrl: './settings-card.component.scss',
  imports: [
    Menubar,
    ShadedCardComponent
  ]
})
export class SettingsCardComponent {
  readonly icon = input<string>();
  readonly title = input.required<string>();
  readonly description = input<string>();
}
