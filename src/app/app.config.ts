import { ApplicationConfig, provideAppInitializer, provideZoneChangeDetection } from "@angular/core";
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { provideRouter, withComponentInputBinding, withRouterConfig } from "@angular/router";

import { providePrimeNG } from 'primeng/config';
import { routes } from "./app.routes";
// import { initializeClipboardListener } from "./initializers/clipboard-listener.initializer";
import { AuraBluePreset } from "./theming/presets/aura-blue.preset";

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes,
      withRouterConfig({ paramsInheritanceStrategy: 'always' }),
      withComponentInputBinding()
    ),
    provideAppInitializer(appInitializer),
    provideAnimationsAsync(),
    providePrimeNG({
      theme: {
        preset: AuraBluePreset
      },
      ripple: true
    })
  ]
};


async function appInitializer() {
  // await initializeTrayIconAsync();
  // initializeClipboardListener();
}
