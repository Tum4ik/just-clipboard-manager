import { Injectable } from '@angular/core';
import { PrimeNG } from 'primeng/config';
import { BehaviorSubject } from 'rxjs';
import { SettingsService, ThemeMode } from './settings.service';

const PREFERS_COLOR_SCHEME_DARK = '(prefers-color-scheme: dark)';
const DARK_MODE_SELECTOR = 'dark-mode';

@Injectable({ providedIn: 'root' })
export class ThemeService {
  constructor(
    private readonly settingsService: SettingsService,
    private readonly primeNg: PrimeNG,
  ) { }

  private isInitialized = false;

  private themeChangedSubject = new BehaviorSubject<'light' | 'dark'>('dark');
  themeChanged$ = this.themeChangedSubject.asObservable();

  get isDarkMode(): boolean {
    const isSystemDarkModeSelector = this.primeNg.theme().options.darkModeSelector === 'system';
    const prefersColorSchemeDark = window.matchMedia(PREFERS_COLOR_SCHEME_DARK).matches;
    const containsDarkModeSelector = document.documentElement.classList.contains(DARK_MODE_SELECTOR);
    return (isSystemDarkModeSelector && prefersColorSchemeDark) || containsDarkModeSelector;
  }


  async initAsync(): Promise<void> {
    if (this.isInitialized) {
      return;
    }

    this.isInitialized = true;

    window.matchMedia(PREFERS_COLOR_SCHEME_DARK).addEventListener('change', e => {
      this.notifyThemeChanged();
    });
    await this.settingsService.onThemeModeChanged(mode => {
      this.setMode(mode!);
    });

    const mode = await this.settingsService.getThemeModeAsync();
    this.setMode(mode);
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
    this.themeChangedSubject.next(this.isDarkMode ? 'dark' : 'light');
  }
}
