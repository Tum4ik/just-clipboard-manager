import { Component } from '@angular/core';
import { MenuItem } from 'primeng/api';
import { NavigationView } from '../navigation-view/navigation-view';

@Component({
  selector: 'jcm-settings-navigation-view',
  template: '',
})
export class SettingsNavigationViewComponent extends NavigationView {
  items: MenuItem[] = [
    {
      label: 'settings..general',
      icon: 'tune',
      routerLink: 'general'
    },
    {
      label: 'settings..interface',
      icon: 'display_settings',
      routerLink: 'interface'
    },
    {
      label: 'settings..paste-window',
      icon: 'wysiwyg',
      routerLink: 'paste-window'
    },
    {
      label: 'settings..hot-keys',
      icon: 'keyboard',
      routerLink: 'hot-keys'
    }
  ];
}
