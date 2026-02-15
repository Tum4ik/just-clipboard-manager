import { Component } from '@angular/core';
import { NavigationMenuItem, NavigationView } from '../navigation-view/navigation-view';
import { GeneralSettingsComponent } from './components/general-settings/general-settings.component';
import { HotKeysSettingsComponent } from './components/hot-keys-settings/hot-keys-settings.component';
import { InterfaceSettingsComponent } from './components/interface-settings/interface-settings.component';
import { PasteWindowSettingsComponent } from './components/paste-window-settings/paste-window-settings.component';

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
      component: GeneralSettingsComponent
    },
    {
      id: 'interface',
      label: 'settings..interface',
      icon: 'display_settings',
      component: InterfaceSettingsComponent
    },
    {
      id: 'paste-window',
      label: 'settings..paste-window',
      icon: 'wysiwyg',
      component: PasteWindowSettingsComponent
    },
    {
      id: 'hot-keys',
      label: 'settings..hot-keys',
      icon: 'keyboard',
      component: HotKeysSettingsComponent
    }
  ];
}
