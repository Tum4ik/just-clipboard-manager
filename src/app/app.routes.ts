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
      ClipboardListener,
      AppTray,
      PasteDataService,
      GlobalShortcutsService,
      PasteWindowService,
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
        loadComponent: () => import('./shells/main-window/components/navigation-view/navigation-view.component').then(c => c.NavigationViewComponent)
      },
      {
        path: 'plugins',
        loadComponent: () => import('./shells/main-window/components/navigation-view/navigation-view.component').then(c => c.NavigationViewComponent)
      }
    ]
  }
];
