import { ApplicationConfig, provideZoneChangeDetection } from "@angular/core";
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { provideRouter, withComponentInputBinding, withRouterConfig } from "@angular/router";
import { providePrimeNG } from 'primeng/config';
import { routes } from "./app.routes";
import { AuraBluePreset } from "./theming/presets/aura-blue.preset";

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes,
      withRouterConfig({ paramsInheritanceStrategy: 'always' }),
      withComponentInputBinding()
    ),
    provideAnimationsAsync(),
    providePrimeNG({
      theme: {
        preset: AuraBluePreset
      },
      ripple: true
    })
  ]
};
