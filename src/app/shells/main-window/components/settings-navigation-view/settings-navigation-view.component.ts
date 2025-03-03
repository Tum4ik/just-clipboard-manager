import { Component } from '@angular/core';
import { MenuItem } from 'primeng/api';
import { NavigationViewComponent } from '../navigation-view/navigation-view.component';

@Component({
  selector: 'jcm-settings-navigation-view',
  templateUrl: './settings-navigation-view.component.html',
  styleUrl: './settings-navigation-view.component.scss',
  imports: [
    NavigationViewComponent
  ]
})
export class SettingsNavigationViewComponent {
  items: MenuItem[] = [
    {
      label: 'general',
    },
    {
      label: 'interface',
    },
    {
      label: 'paste-window',
    },
    {
      label: 'hot-keys',
    }
  ];
}
