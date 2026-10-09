import { Component } from '@angular/core';
import { Select } from 'primeng/select';
import { SettingCard } from '../setting-card/setting-card';

@Component({
  selector: 'jcm-interface-settings',
  templateUrl: './interface-settings.html',
  styleUrl: './interface-settings.css',
  imports: [
    SettingCard,
    Select,
  ],
})
export class InterfaceSettings {

}
