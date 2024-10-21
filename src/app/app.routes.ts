import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: 'index.html',
    loadComponent: () => import('./window-router.component').then(c => c.WindowRouterComponent)
  },
  {
    path: 'paste-window',
    loadComponent: () => import('./paste-window/paste-window.component').then(c => c.PasteWindowComponent)
  },
  {
    path: 'main-window',
    loadComponent: () => import('./main-window/main-window.component').then(c => c.MainWindowComponent)
  }
];
