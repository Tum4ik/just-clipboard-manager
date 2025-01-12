import { Routes } from "@angular/router";

export const routes: Routes = [
  {
    path: 'paste-window',
    loadComponent: () => import('./shells/paste-window/paste-window.component').then(c => c.PasteWindowComponent)
  },
  {
    path: 'main-window',
    loadComponent: () => import('./shells/main-window/main-window.component').then(c => c.MainWindowComponent)
  }
];
