import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter, withComponentInputBinding, withHashLocation } from '@angular/router';
import { providePrimeNG } from 'primeng/config';
import primeuiLicense from '../../primeui-license.json';
import { routes } from './app.routes';
import { getThemePreset } from './theming/get-theme-preset';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes, withComponentInputBinding(), withHashLocation()),
    providePrimeNG({
      license: `${primeuiLicense.licenseKey}`,
      theme: {
        preset: getThemePreset(),
        options: {
          cssLayer: {
            name: 'primeng',
            order: 'theme, base, primeng'
          }
        }
      },
      ripple: true
    })
  ]
};
