import { Injectable } from '@angular/core';
import { emit, Event, listen } from '@tauri-apps/api/event';
import { Theme } from '@tauri-apps/api/window';
import { PrimeNG } from 'primeng/config';
import { BehaviorSubject } from 'rxjs';
import { SettingsService, ThemeMode } from './settings.service';

const PREFERS_COLOR_SCHEME_DARK = '(prefers-color-scheme: dark)';
const DARK_MODE_SELECTOR = 'dark-mode';
const THEME_MODE_CHANGED_EVENT_NAME = 'theme-mode-changed-event';

@Injectable({ providedIn: 'root' })
export class ThemeService {
  constructor(
    private readonly settingsService: SettingsService,
    private readonly primeNg: PrimeNG,
  ) {
    this.settingsService.getThemeModeAsync().then(themeMode => {
      this.setMode(themeMode);
    });
    window.matchMedia(PREFERS_COLOR_SCHEME_DARK).addEventListener('change', e => {
      this.notifyThemeChanged();
    });
    listen<ThemeMode>(THEME_MODE_CHANGED_EVENT_NAME, this.onThemeGloballyChanged.bind(this));
  }


  private theme = new BehaviorSubject<Theme>('dark');
  theme$ = this.theme.asObservable();

  async setThemeModeAsync(themeMode: ThemeMode) {
    await this.settingsService.setThemeModeAsync(themeMode);
    await emit<ThemeMode>(THEME_MODE_CHANGED_EVENT_NAME, themeMode);
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
    this.notifyThemeChanged();
  }


  private notifyThemeChanged() {
    this.theme.next(this.isDarkMode ? 'dark' : 'light');
  }
}
