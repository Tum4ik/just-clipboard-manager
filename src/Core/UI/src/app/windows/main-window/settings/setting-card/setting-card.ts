import { Component, input } from '@angular/core';
import { MaterialIcon } from '@app/shared/material-icon/material-icon';
import { Card } from 'primeng/card';

@Component({
  selector: 'jcm-setting-card',
  templateUrl: './setting-card.html',
  styleUrl: './setting-card.css',
  imports: [
    Card,
    MaterialIcon,
  ],
})
export class SettingCard {
  readonly materialIconName = input.required<string>();
  readonly settingTitle = input.required<string>();
  readonly settingDescription = input.required<string>();
}
