import { Routes } from "@angular/router";
import { SettingsService } from "./core/services/settings.service";
import { clipboardListenerResolver } from "./shells/paste-window/resolvers/clipboard-listener.resolver";
import { globalShortcutsResolver } from "./shells/paste-window/resolvers/global-shortcuts.resolver";
import { trayIconResolver } from "./shells/paste-window/resolvers/tray-icon.resolver";

export const routes: Routes = [
  {
    path: 'paste-window',
    loadComponent: () => import('./shells/paste-window/paste-window.component').then(c => c.PasteWindowComponent),
    providers: [
      SettingsService
    ],
    resolve: {
      trayIcon: trayIconResolver,
      clipboardListener: clipboardListenerResolver,
      globalShortcuts: globalShortcutsResolver
    }
  },
  {
    path: 'main-window',
    loadComponent: () => import('./shells/main-window/main-window.component').then(c => c.MainWindowComponent),
    providers: [
      SettingsService
    ]
  }
];
