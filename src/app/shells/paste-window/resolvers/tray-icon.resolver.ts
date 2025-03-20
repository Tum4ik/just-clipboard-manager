import { inject } from '@angular/core';
import { ResolveFn } from '@angular/router';
import { AppTray } from '../../tray/app-tray';

export const trayIconResolver: ResolveFn<void> = async (route, state) => {
  await inject(AppTray).initAsync();
};
