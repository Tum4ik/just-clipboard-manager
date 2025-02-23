import { Routes } from "@angular/router";
import { clipboardListenerResolver } from "./shells/paste-window/resolvers/clipboard-listener.resolver";
import { globalShortcutsResolver } from "./shells/paste-window/resolvers/global-shortcuts.resolver";
import { trayIconResolver } from "./shells/paste-window/resolvers/tray-icon.resolver";
import { ClipboardListener } from "./shells/paste-window/services/clipboard-listener.service";
import { GlobalShortcutsService } from "./shells/paste-window/services/global-shortcuts.service";
import { PasteDataService } from "./shells/paste-window/services/paste-data.service";
import { PasteWindowVisibilityService } from "./shells/paste-window/services/paste-window-visibility.service";
import { AppTray } from "./shells/tray/app-tray";
import { pasteWindowVisibilityResolver } from "./shells/paste-window/resolvers/paste-window-visibility.resolver";

export const routes: Routes = [
  {
    path: 'paste-window',
    loadComponent: () => import('./shells/paste-window/paste-window.component').then(c => c.PasteWindowComponent),
    providers: [
      ClipboardListener,
      AppTray,
      PasteDataService,
      GlobalShortcutsService,
      PasteWindowVisibilityService,
    ],
    resolve: {
      trayIcon: trayIconResolver,
      clipboardListener: clipboardListenerResolver,
      globalShortcuts: globalShortcutsResolver,
      pasteWindowVisibility: pasteWindowVisibilityResolver,
    }
  },
  {
    path: 'main-window',
    loadComponent: () => import('./shells/main-window/main-window.component').then(c => c.MainWindowComponent),
  }
];
