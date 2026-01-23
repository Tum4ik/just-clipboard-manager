import { Injectable } from '@angular/core';
import { Event } from '@tauri-apps/api/event';
import { PrimeNG } from 'primeng/config';
import { BehaviorSubject } from 'rxjs';
import { GlobalStateService } from './base/global-state-service';
import { SettingsService, ThemeMode } from './settings.service';

const PREFERS_COLOR_SCHEME_DARK = '(prefers-color-scheme: dark)';
const DARK_MODE_SELECTOR = 'dark-mode';

@Injectable({ providedIn: 'root' })
export class ThemeService extends GlobalStateService {
  constructor(
    private readonly settingsService: SettingsService,
    private readonly primeNg: PrimeNG,
  ) {
    super();

    this.settingsService.getThemeModeAsync().then(themeMode => {
      this.setMode(themeMode);
    });
    window.matchMedia(PREFERS_COLOR_SCHEME_DARK).addEventListener('change', e => {
      this.notifyThemeChanged();
    });
  }

  private readonly themeModeGlobalSetter = this.registerGlobalObservable(
    'theme-mode-changed-event',
    this.onThemeGloballyChanged.bind(this)
  );

  private themeMode = new BehaviorSubject<ThemeMode>('system');
  themeMode$ = this.themeMode.asObservable();

  private isDarkTheme = new BehaviorSubject<boolean>(true);
  isDarkTheme$ = this.isDarkTheme.asObservable();

  async setThemeModeAsync(themeMode: ThemeMode) {
    await this.settingsService.setThemeModeAsync(themeMode);
    await this.themeModeGlobalSetter.setAsync(themeMode);
  }

  private get isDarkMode(): boolean {
    const isSystemDarkModeSelector = this.primeNg.theme().options.darkModeSelector === 'system';
    const prefersColorSchemeDark = window.matchMedia(PREFERS_COLOR_SCHEME_DARK).matches;
    const containsDarkModeSelector = document.documentElement.classList.contains(DARK_MODE_SELECTOR);
    return (isSystemDarkModeSelector && prefersColorSchemeDark) || containsDarkModeSelector;
  }


  private onThemeGloballyChanged(e: Event<ThemeMode>) {
    this.setMode(e.payload);
  }


  private setMode(mode: ThemeMode) {
    const currentTheme = this.primeNg.theme();
    const htmlElement = document.querySelector('html');
    switch (mode) {
      case 'system':
        currentTheme.options.darkModeSelector = 'system';
        htmlElement?.classList?.remove(DARK_MODE_SELECTOR);
        break;
      case 'light':
        currentTheme.options.darkModeSelector = 'none';
        htmlElement?.classList?.remove(DARK_MODE_SELECTOR);
        break;
      case 'dark':
        currentTheme.options.darkModeSelector = `.${DARK_MODE_SELECTOR}`;
        htmlElement?.classList?.add(DARK_MODE_SELECTOR);
        break;
    }

    this.primeNg.onThemeChange(currentTheme);
    this.themeMode.next(mode);
    this.notifyThemeChanged();
  }


  private notifyThemeChanged() {
    this.isDarkTheme.next(this.isDarkMode);
  }
}
