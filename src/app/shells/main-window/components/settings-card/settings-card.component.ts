import { Component, input } from '@angular/core';
import { Card } from 'primeng/card';
import { Menubar } from 'primeng/menubar';

@Component({
  selector: 'jcm-settings-card',
  templateUrl: './settings-card.component.html',
  styleUrl: './settings-card.component.scss',
  imports: [
    Card,
    Menubar
  ]
})
export class SettingsCardComponent {
  readonly icon = input<string>();
  readonly title = input.required<string>();
  readonly description = input<string>();
}
