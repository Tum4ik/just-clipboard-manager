import { provideHttpClient, withFetch } from "@angular/common/http";
import { ApplicationConfig, ErrorHandler, inject, provideAppInitializer, provideBrowserGlobalErrorListeners } from "@angular/core";
import { MAT_TOOLTIP_DEFAULT_OPTIONS } from "@angular/material/tooltip";
import { provideRouter, withComponentInputBinding, withRouterConfig } from "@angular/router";
import { provideTranslateService, TranslateService } from "@ngx-translate/core";
import { provideTranslateHttpLoader } from '@ngx-translate/http-loader';
import { providePrimeNG } from 'primeng/config';
import { firstValueFrom } from "rxjs";
import { routes } from "./app.routes";
import { TOOLTIP_OPTIONS } from "./core/config/tooltip.config";
import { ClipsAutoDeleteService } from "./core/services/clips-auto-delete.service";
import { MonitoringService } from "./core/services/monitoring.service";
import { PluginsService } from "./core/services/plugins.service";
import { SettingsService } from "./core/services/settings.service";
import { ThemeService } from "./core/services/theme.service";
import { registerSvgIcons } from "./initializers/register-svg-icons";
import { AuraBluePreset } from "./theming/presets/aura-blue.preset";

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes,
      withRouterConfig({ paramsInheritanceStrategy: 'always' }),
      withComponentInputBinding()
    ),
    providePrimeNG({
      theme: {
        preset: AuraBluePreset,
        options: {
          darkModeSelector: 'system'
        }
      },
      ripple: true
    }),
    provideHttpClient(withFetch()),
    provideTranslateService({
      fallbackLang: 'en',
      loader: provideTranslateHttpLoader({ prefix: './assets/i18n/', suffix: '.json' })
    }),
    provideAppInitializer(async () => {

      inject(ThemeService);
      const settingsService = inject(SettingsService);
      const translateService = inject(TranslateService);
      const pluginsService = inject(PluginsService);
      const clipsAutoDeleteService = inject(ClipsAutoDeleteService);
      registerSvgIcons();

      translateService.addLangs(['en', 'uk']);
      const lang = await settingsService.getLanguageAsync();
      await firstValueFrom(translateService.use(lang));
      await settingsService.onLanguageChanged(l => translateService.use(l ?? 'en'));

      await pluginsService.initAsync();
      await clipsAutoDeleteService.deleteOutdatedClipsAsync();
    }),
    { provide: ErrorHandler, useExisting: MonitoringService },
    { provide: MAT_TOOLTIP_DEFAULT_OPTIONS, useValue: TOOLTIP_OPTIONS },
  ]
};
