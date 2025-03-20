import { inject } from '@angular/core';
import { ResolveFn } from '@angular/router';
import { GlobalShortcutsService } from '../services/global-shortcuts.service';

export const globalShortcutsResolver: ResolveFn<void> = async (route, state) => {
  await inject(GlobalShortcutsService).initAsync();
};
