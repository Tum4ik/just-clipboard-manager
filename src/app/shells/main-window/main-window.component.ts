import { Tab, TabContent, TabList, TabPanel, Tabs } from '@angular/aria/tabs';
import { AsyncPipe, NgComponentOutlet } from '@angular/common';
import { Component, computed, input, Type } from '@angular/core';
import { GoogleIcon } from '@app/core/components/google-icon/google-icon';
import { TitleBarComponent } from '@app/core/components/title-bar/title-bar.component';
import { TranslatePipe } from '@ngx-translate/core';
import { ButtonDirective } from 'primeng/button';
import { Panel } from 'primeng/panel';
import { Ripple } from 'primeng/ripple';

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
    AsyncPipe,
  ]
})
export class MainWindowComponent {
  protected readonly tabs: MainWindowTab[] = [
    {
      id: MainWindowTabId.settings,
      label: 'settings',
      icon: 'settings',
      component: import('./components/settings-navigation-view/settings-navigation-view.component')
        .then(c => c.SettingsNavigationViewComponent)
    },
    {
      id: MainWindowTabId.plugins,
      label: 'plugins',
      icon: 'extension',
      component: import('./components/plugins-navigation-view/plugins-navigation-view.component')
        .then(c => c.PluginsNavigationViewComponent)
    },
    {
      id: MainWindowTabId.about,
      label: 'about',
      icon: 'info',
      component: import('./components/about-view/about-view.component')
        .then(c => c.AboutViewComponent)
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

  protected isNavigationView(type: Type<unknown> | null): boolean {
    return !!(type as any)?.isNavigationView;
  }
}

export interface MainWindowTab {
  id: MainWindowTabId;
  label: string;
  icon: string;
  component: Promise<Type<unknown> | null>;
}

export enum MainWindowTabId {
  settings = 'settings',
  plugins = 'plugins',
  about = 'about',
}
