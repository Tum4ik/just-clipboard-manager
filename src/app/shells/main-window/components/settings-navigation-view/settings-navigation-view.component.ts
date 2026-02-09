import { Component } from '@angular/core';
import { NavigationMenuItem, NavigationView } from '../navigation-view/navigation-view';

@Component({
  selector: 'jcm-settings-navigation-view',
  template: '',
})
export class SettingsNavigationViewComponent extends NavigationView {
  protected override readonly items: NavigationMenuItem[] = [
    {
      id: 'general',
      label: 'settings..general',
      icon: 'tune',
      component: import('./components/general-settings/general-settings.component')
        .then(c => c.GeneralSettingsComponent)
    },
    {
      id: 'interface',
      label: 'settings..interface',
      icon: 'display_settings',
      component: import('./components/interface-settings/interface-settings.component')
        .then(c => c.InterfaceSettingsComponent)
    },
    {
      id: 'paste-window',
      label: 'settings..paste-window',
      icon: 'wysiwyg',
      component: import('./components/paste-window-settings/paste-window-settings.component')
        .then(c => c.PasteWindowSettingsComponent)
    },
    {
      id: 'hot-keys',
      label: 'settings..hot-keys',
      icon: 'keyboard',
      component: import('./components/hot-keys-settings/hot-keys-settings.component')
        .then(c => c.HotKeysSettingsComponent)
    }
  ];
}
