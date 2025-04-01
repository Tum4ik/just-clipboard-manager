import { Routes } from "@angular/router";
import { clipboardListenerResolver } from "./shells/paste-window/resolvers/clipboard-listener.resolver";
import { globalShortcutsResolver } from "./shells/paste-window/resolvers/global-shortcuts.resolver";
import { pasteWindowServiceResolver } from "./shells/paste-window/resolvers/paste-window-visibility.resolver";
import { trayIconResolver } from "./shells/paste-window/resolvers/tray-icon.resolver";
import { ClipboardListener } from "./shells/paste-window/services/clipboard-listener.service";
import { GlobalShortcutsService } from "./shells/paste-window/services/global-shortcuts.service";
import { PasteDataService } from "./shells/paste-window/services/paste-data.service";
import { PasteWindowService } from "./shells/paste-window/services/paste-window.service";
import { AppTray } from "./shells/tray/app-tray";

export const routes: Routes = [
  {
    path: 'paste-window',
    loadComponent: () => import('./shells/paste-window/paste-window.component').then(c => c.PasteWindowComponent),
    providers: [
      AppTray,
      ClipboardListener,
      GlobalShortcutsService,
      PasteWindowService,
      PasteDataService,
    ],
    resolve: {
      trayIcon: trayIconResolver,
      clipboardListener: clipboardListenerResolver,
      globalShortcuts: globalShortcutsResolver,
      pasteWindow: pasteWindowServiceResolver,
    }
  },
  {
    path: 'main-window',
    loadComponent: () => import('./shells/main-window/main-window.component').then(c => c.MainWindowComponent),
    children: [
      {
        path: 'settings',
        loadComponent: () =>
          import('./shells/main-window/components/settings-navigation-view/settings-navigation-view.component')
            .then(c => c.SettingsNavigationViewComponent),
        children: [
          {
            path: 'general',
            loadComponent: () =>
              import('./shells/main-window/components/settings-navigation-view/components/general-settings/general-settings.component')
                .then(c => c.GeneralSettingsComponent)
          },
          {
            path: 'interface',
            loadComponent: () =>
              import('./shells/main-window/components/settings-navigation-view/components/interface-settings/interface-settings.component')
                .then(c => c.InterfaceSettingsComponent)
          },
          {
            path: 'paste-window',
            loadComponent: () =>
              import('./shells/main-window/components/settings-navigation-view/components/paste-window-settings/paste-window-settings.component')
                .then(c => c.PasteWindowSettingsComponent)
          },
          {
            path: 'hot-keys',
            loadComponent: () =>
              import('./shells/main-window/components/settings-navigation-view/components/hot-keys-settings/hot-keys-settings.component')
                .then(c => c.HotKeysSettingsComponent)
          }
        ]
      },
      {
        path: 'plugins',
        loadComponent: () =>
          import('./shells/main-window/components/plugins-navigation-view/plugins-navigation-view.component')
            .then(c => c.PluginsNavigationViewComponent),
        children: [
          {
            path: 'pipeline',
            loadComponent: () =>
              import('./shells/main-window/components/plugins-navigation-view/components/plugins-pipeline/plugins-pipeline.component')
                .then(c => c.PluginsPipelineComponent)
          }
        ]
      },
      {
        path: 'about',
        loadComponent: () =>
          import('./shells/main-window/components/about-view/about-view.component')
            .then(c => c.AboutViewComponent)
      }
    ]
  }
];
