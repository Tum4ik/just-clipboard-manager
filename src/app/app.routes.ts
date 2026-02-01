import { Routes } from "@angular/router";
import { GlobalShortcutsSettingService } from "./shells/main-window/services/global-shortcuts-setting.service";
import { clipboardListenerResolver } from "./shells/paste-window/resolvers/clipboard-listener.resolver";
import { globalShortcutsResolver } from "./shells/paste-window/resolvers/global-shortcuts.resolver";
import { pasteWindowServiceResolver } from "./shells/paste-window/resolvers/paste-window-visibility.resolver";
import { trayIconResolver } from "./shells/paste-window/resolvers/tray-icon.resolver";
import { ClipboardListener } from "./shells/paste-window/services/clipboard-listener.service";
import { GlobalShortcutsRegistrationService } from "./shells/paste-window/services/global-shortcuts-registration.service";
import { PasteDataService } from "./shells/paste-window/services/paste-data.service";
import { PasteWindowClipsService } from "./shells/paste-window/services/paste-window-clips.service";
import { PasteWindowService } from "./shells/paste-window/services/paste-window.service";
import { AppTray } from "./shells/tray/app-tray";

export const routes: Routes = [
  {
    path: 'paste-window',
    loadComponent: () => import('./shells/paste-window/paste-window.component').then(c => c.PasteWindowComponent),
    providers: [
      AppTray,
      ClipboardListener,
      GlobalShortcutsRegistrationService,
      PasteWindowService,
      PasteWindowClipsService,
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
    path: 'full-data-preview/:clipId',
    loadComponent: () => import('./shells/paste-window/components/clip-full-data-preview/clip-full-data-preview')
      .then(c => c.ClipFullDataPreview)
  },
  {
    path: 'main-window',
    loadComponent: () => import('./shells/main-window/main-window.component').then(c => c.MainWindowComponent),
    providers: [
      GlobalShortcutsSettingService,
    ],
  }
];
