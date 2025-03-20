import { Injectable } from '@angular/core';
import { Theme, useTheme } from '@primeng/themes';
import { BehaviorSubject } from 'rxjs';
import { SettingsService, ThemeMode } from './settings.service';

const PREFERS_COLOR_SCHEME_DARK = '(prefers-color-scheme: dark)';
const DARK_MODE_SELECTOR = 'dark-mode';

@Injectable({ providedIn: 'root' })
export class ThemeService {
  constructor(
    private readonly settingsService: SettingsService
  ) { }

  private isInitialized = false;

  private themeChangedSubject = new BehaviorSubject<'light' | 'dark'>(this.isDarkMode ? 'dark' : 'light');
  themeChanged$ = this.themeChangedSubject.asObservable();

  get isDarkMode(): boolean {
    return (Theme.getTheme().options.darkModeSelector === 'system' && window.matchMedia(PREFERS_COLOR_SCHEME_DARK).matches)
      || (window.document.documentElement.classList.contains(DARK_MODE_SELECTOR));
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
    const currentTheme = Theme.getTheme();
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
    useTheme(currentTheme);
    this.notifyThemeChanged();
  }


  private notifyThemeChanged() {
    this.themeChangedSubject.next(this.isDarkMode ? 'dark' : 'light');
  }
}
