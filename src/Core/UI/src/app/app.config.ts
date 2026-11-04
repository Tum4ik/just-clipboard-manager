import { ApplicationConfig, inject, provideAppInitializer, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter, withComponentInputBinding, withHashLocation } from '@angular/router';
import { provideTranslateLoader, provideTranslateService, TranslateService } from "@ngx-translate/core";
import { providePrimeNG } from 'primeng/config';
import primeuiLicense from '../../primeui-license.json';
import { routes } from './app.routes';
import { BackendProcessEventsService } from './services/backend-process-events-service';
import { CommandsService } from './services/commands-service';
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
      const commandsService = inject(CommandsService);
      const translate = inject(TranslateService);
      const backendProcessEventsService = inject(BackendProcessEventsService);

      translate.addLangs(['en', 'uk']);
      const selectedLanguage = await commandsService.getSelectedLanguageAsync();
      translate.use(selectedLanguage);
      backendProcessEventsService.languageChanged.subscribe(payload => {
        translate.use(payload.languageCode);
      });
    }),
  ]
};
