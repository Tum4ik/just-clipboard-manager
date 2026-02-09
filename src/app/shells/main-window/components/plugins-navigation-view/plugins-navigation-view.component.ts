import { Component } from '@angular/core';
import { NavigationMenuItem, NavigationView } from '../navigation-view/navigation-view';

@Component({
  selector: 'jcm-plugins-navigation-view',
  template: '',
})
export class PluginsNavigationViewComponent extends NavigationView {
  protected override readonly items: NavigationMenuItem[] = [
    {
      id: 'installed',
      label: 'installed',
      icon: 'list_alt_check',
      component: import('./components/installed-plugins/installed-plugins').then(c => c.InstalledPlugins)
    },
    {
      id: 'search',
      label: 'search',
      icon: 'manage_search',
      component: import('./components/search-plugins/search-plugins.component').then(c => c.SearchPluginsComponent)
    },
    {
      id: 'data-processing-pipeline',
      label: 'data-processing-pipeline',
      icon: 'move_down',
      component: import('./components/plugins-pipeline/plugins-pipeline.component').then(c => c.PluginsPipelineComponent)
    },
  ];
}
