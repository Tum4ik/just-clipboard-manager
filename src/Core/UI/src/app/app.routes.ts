import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: 'paste-window',
    loadComponent: () => import('./windows/paste-window/paste-window').then(c => c.PasteWindow),
  },
  {
    path: 'main-window',
    loadComponent: () => import('./windows/main-window/main-window').then(c => c.MainWindow),
  }
];
