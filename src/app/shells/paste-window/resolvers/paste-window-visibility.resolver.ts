import { inject } from '@angular/core';
import { ResolveFn } from '@angular/router';
import { PasteWindowVisibilityService } from '../services/paste-window-visibility.service';

export const pasteWindowVisibilityResolver: ResolveFn<void> = async (route, state) => {
  await inject(PasteWindowVisibilityService).initAsync();
};
