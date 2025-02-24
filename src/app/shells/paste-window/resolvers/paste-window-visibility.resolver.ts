import { inject } from '@angular/core';
import { ResolveFn } from '@angular/router';
import { PasteWindowService } from '../services/paste-window.service';

export const pasteWindowServiceResolver: ResolveFn<void> = async (route, state) => {
  await inject(PasteWindowService).initAsync();
};
