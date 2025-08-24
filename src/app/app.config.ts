import { HttpClient, provideHttpClient, withFetch } from "@angular/common/http";
import { ApplicationConfig, ErrorHandler, inject, provideAppInitializer, provideBrowserGlobalErrorListeners, provideZoneChangeDetection } from "@angular/core";
import { MAT_TOOLTIP_DEFAULT_OPTIONS } from "@angular/material/tooltip";
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { provideRouter, RouteReuseStrategy, withComponentInputBinding, withRouterConfig } from "@angular/router";
import { provideTranslateService, TranslateLoader, TranslateService } from "@ngx-translate/core";
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { providePrimeNG } from 'primeng/config';
import { firstValueFrom } from "rxjs";
import { routes } from "./app.routes";
import { TOOLTIP_OPTIONS } from "./core/config/tooltip.config";
import { EnvironmentService } from "./core/services/environment.service";
import { MonitoringService } from "./core/services/monitoring.service";
import { PluginsService } from "./core/services/plugins.service";
import { SettingsService } from "./core/services/settings.service";
import { registerSvgIcons } from "./initializers/register-svg-icons";
import { AppRouteReuseStrategy } from "./router/app-route-reuse-strategy";
import { AuraBluePreset } from "./theming/presets/aura-blue.preset";

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes,
      withRouterConfig({ paramsInheritanceStrategy: 'always' }),
      withComponentInputBinding()
    ),
    provideAnimationsAsync(),
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
      defaultLanguage: 'en',
      loader: {
        provide: TranslateLoader,
        useFactory: createTranslateLoader,
        deps: [HttpClient]
      }
    }),
    provideAppInitializer(async () => {
      registerSvgIcons();

      const environmentService = inject(EnvironmentService);
      const settings = inject(SettingsService);
      const translate = inject(TranslateService);
      const pluginsService = inject(PluginsService);

      await environmentService.initAsync();

      translate.addLangs(['en', 'uk']);
      await firstValueFrom(translate.use(await settings.getLanguageAsync()));
      await settings.onLanguageChanged(l => translate.use(l ?? 'en'));

      await pluginsService.initAsync();
    }),
    { provide: ErrorHandler, useExisting: MonitoringService },
    { provide: RouteReuseStrategy, useClass: AppRouteReuseStrategy },
    { provide: MAT_TOOLTIP_DEFAULT_OPTIONS, useValue: TOOLTIP_OPTIONS },
  ]
};

export function createTranslateLoader(http: HttpClient) {
  return new TranslateHttpLoader(http, './assets/i18n/', '.json');
}
