import { Tab, TabContent, TabList, TabPanel, Tabs } from '@angular/aria/tabs';
import { NgComponentOutlet } from '@angular/common';
import { Component, computed, input, Type } from '@angular/core';
import { GoogleIcon } from '@app/core/components/google-icon/google-icon';
import { TitleBarComponent } from '@app/core/components/title-bar/title-bar.component';
import { TranslatePipe } from '@ngx-translate/core';
import { ButtonDirective } from 'primeng/button';
import { Panel } from 'primeng/panel';
import { Ripple } from 'primeng/ripple';
import { AboutViewComponent } from './components/about-view/about-view.component';
import { PluginsNavigationViewComponent } from './components/plugins-navigation-view/plugins-navigation-view.component';
import { SettingsNavigationViewComponent } from './components/settings-navigation-view/settings-navigation-view.component';

@Component({
  selector: 'jcm-main-window',
  templateUrl: './main-window.component.html',
  styleUrl: './main-window.component.scss',
  imports: [
    TitleBarComponent,
    Panel,
    ButtonDirective,
    NgComponentOutlet,
    Ripple,
    TranslatePipe,
    GoogleIcon,
    TabList, Tab, Tabs, TabPanel, TabContent,
  ]
})
export class MainWindowComponent {
  protected readonly tabs: MainWindowTab[] = [
    {
      id: MainWindowTabId.settings,
      label: 'settings',
      icon: 'settings',
      component: SettingsNavigationViewComponent
    },
    {
      id: MainWindowTabId.plugins,
      label: 'plugins',
      icon: 'extension',
      component: PluginsNavigationViewComponent
    },
    {
      id: MainWindowTabId.about,
      label: 'about',
      icon: 'info',
      component: AboutViewComponent
    },
  ];

  readonly topLevelTabId = input<MainWindowTabId>(this.tabs[0].id);
  readonly nestedLevelTabId = input<string>();

  protected readonly selectedTabId = computed(() => {
    const tabId = this.topLevelTabId();
    if (Object.values(MainWindowTabId).includes(tabId)) {
      return tabId;
    }
    return MainWindowTabId.settings;
  });
}

export interface MainWindowTab {
  id: MainWindowTabId;
  label: string;
  icon: string;
  component: Type<any> | null;
}

export enum MainWindowTabId {
  settings = 'settings',
  plugins = 'plugins',
  about = 'about',
}
