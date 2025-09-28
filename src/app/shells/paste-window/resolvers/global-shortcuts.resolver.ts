import { inject } from '@angular/core';
import { ResolveFn } from '@angular/router';
import { GlobalShortcutsRegistrationService } from '../services/global-shortcuts-registration.service';

export const globalShortcutsResolver: ResolveFn<void> = async (route, state) => {
  await inject(GlobalShortcutsRegistrationService).initAsync();
};
