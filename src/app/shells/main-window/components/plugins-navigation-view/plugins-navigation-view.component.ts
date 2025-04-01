import { Component } from '@angular/core';
import { MenuItem } from 'primeng/api';
import { NavigationViewComponent } from '../navigation-view/navigation-view.component';

@Component({
  selector: 'jcm-plugins-navigation-view',
  templateUrl: './plugins-navigation-view.component.html',
  styleUrl: './plugins-navigation-view.component.scss',
  imports: [
    NavigationViewComponent
  ]
})
export class PluginsNavigationViewComponent {
  items: MenuItem[] = [
    {
      label: 'data-processing-pipeline',
      icon: 'move_down',
      routerLink: 'pipeline'
    }
  ];
}
