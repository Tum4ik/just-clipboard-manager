import { Component, input } from '@angular/core';
import { GoogleIcon } from "@app/core/components/google-icon/google-icon";
import { Menubar } from 'primeng/menubar';
import { ShadedCardComponent } from "../shaded-card/shaded-card.component";

@Component({
  selector: 'jcm-settings-card',
  templateUrl: './settings-card.component.html',
  styleUrl: './settings-card.component.scss',
  imports: [
    Menubar,
    ShadedCardComponent,
    GoogleIcon
  ]
})
export class SettingsCardComponent {
  readonly icon = input<string>();
  readonly title = input.required<string>();
  readonly description = input<string>();
}
