import { ApplicationConfig, inject, provideAppInitializer, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter, withComponentInputBinding, withHashLocation } from '@angular/router';
import { provideTranslateLoader, provideTranslateService, TranslateService } from "@ngx-translate/core";
import { providePrimeNG } from 'primeng/config';
import primeuiLicense from '../../primeui-license.json';
import { routes } from './app.routes';
import { AppCommandsService } from './services/app-commands-service';
import { AppEventsService } from './services/app-events-service';
import { TranslateYamlHttpLoader } from './services/translate-yaml-http-loader';
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
    }),
    provideTranslateService({
      fallbackLang: 'en',
      lang: 'en',
      loader: provideTranslateLoader(TranslateYamlHttpLoader),
    }),
    provideAppInitializer(async () => {
      const commandsService = inject(AppCommandsService);
      const translate = inject(TranslateService);
      const appEventsService = inject(AppEventsService);

      translate.addLangs(['en', 'uk']);
      const selectedLanguage = await commandsService.getSelectedLanguageAsync();
      translate.use(selectedLanguage);
      appEventsService.languageChanged.subscribe(payload => {
        translate.use(payload.languageCode);
      });
    }),
  ]
};
