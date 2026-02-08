import { Component } from '@angular/core';
import { NavigationMenuItem, NavigationView } from '../navigation-view/navigation-view';
import { InstalledPlugins } from './components/installed-plugins/installed-plugins';
import { PluginsPipelineComponent } from './components/plugins-pipeline/plugins-pipeline.component';
import { SearchPluginsComponent } from './components/search-plugins/search-plugins.component';

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
      // todo: implement lazy loading for components like in main-window
      component: InstalledPlugins
    },
    {
      id: 'search',
      label: 'search',
      icon: 'manage_search',
      component: SearchPluginsComponent
    },
    {
      id: 'data-processing-pipeline',
      label: 'data-processing-pipeline',
      icon: 'move_down',
      component: PluginsPipelineComponent
    },
  ];
}
