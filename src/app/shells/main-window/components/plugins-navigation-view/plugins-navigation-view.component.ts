import { Component } from '@angular/core';
import { MenuItem } from 'primeng/api';
import { NavigationView } from '../navigation-view/navigation-view';

@Component({
  selector: 'jcm-plugins-navigation-view',
  template: '',
})
export class PluginsNavigationViewComponent extends NavigationView {
  items: MenuItem[] = [
    {
      label: 'installed',
      icon: 'list_alt_check',
      routerLink: 'installed'
    },
    {
      label: 'search',
      icon: 'manage_search',
      routerLink: 'search'
    },
    {
      label: 'data-processing-pipeline',
      icon: 'move_down',
      routerLink: 'pipeline'
    },
  ];
}
