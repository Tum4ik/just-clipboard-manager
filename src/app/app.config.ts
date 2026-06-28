import { provideHttpClient } from "@angular/common/http";
import { ApplicationConfig, ErrorHandler, inject, provideAppInitializer, provideBrowserGlobalErrorListeners } from "@angular/core";
import { MAT_TOOLTIP_DEFAULT_OPTIONS } from "@angular/material/tooltip";
import { provideRouter, withComponentInputBinding, withRouterConfig } from "@angular/router";
import { provideTranslateService } from "@ngx-translate/core";
import { provideTranslateHttpLoader } from '@ngx-translate/http-loader';
import { providePrimeNG } from 'primeng/config';
import { routes } from "./app.routes";
import { TOOLTIP_OPTIONS } from "./core/config/tooltip.config";
import { LanguageSwitchingService } from "./core/services/language-switching-service";
import { MonitoringService } from "./core/services/monitoring.service";
import { PluginsService } from "./core/services/plugins.service";
import { ThemeService } from "./core/services/theme.service";
import { registerSvgIcons } from "./initializers/register-svg-icons";
import { getThemePreset } from "./theming/get-theme-preset";

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes,
      withRouterConfig({ paramsInheritanceStrategy: 'always' }),
      withComponentInputBinding()
    ),
    providePrimeNG({
      theme: {
        preset: getThemePreset('blue'),
        options: {
          darkModeSelector: 'system'
        }
      },
      ripple: true
    }),
    provideHttpClient(),
    provideTranslateService({
      fallbackLang: 'en',
      loader: provideTranslateHttpLoader({ prefix: './assets/i18n/', suffix: '.json' })
    }),
    provideAppInitializer(async () => {
      inject(ThemeService);
      inject(LanguageSwitchingService);
      const pluginsService = inject(PluginsService);
      registerSvgIcons();

      await pluginsService.initAsync();
    }),
    { provide: ErrorHandler, useExisting: MonitoringService },
    { provide: MAT_TOOLTIP_DEFAULT_OPTIONS, useValue: TOOLTIP_OPTIONS },
  ]
};
