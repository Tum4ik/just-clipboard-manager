import { Injectable } from '@angular/core';
import { getThemePreset, PresetColor } from '@app/theming/get-theme-preset';
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

    this.settingsService.themeMode.getAsync().then(this.setMode.bind(this));
    this.settingsService.themePrimaryColor.getAsync().then(this.setPresetColor.bind(this));
    window.matchMedia(PREFERS_COLOR_SCHEME_DARK).addEventListener('change', e => {
      this.notifyThemeChanged();
    });
  }

  private readonly themeModeGlobalSetter = this.registerGlobalObservable(
    'theme-mode-changed-event',
    this.onThemeGloballyChanged.bind(this)
  );

  private readonly themePrimaryColorGlobalSetter = this.registerGlobalObservable(
    'theme-primary-color-changed-event',
    this.onThemeColorGloballyChanged.bind(this)
  );

  private readonly themeMode = new BehaviorSubject<ThemeMode>('system');
  readonly themeMode$ = this.themeMode.asObservable();

  private readonly themePrimaryColor = new BehaviorSubject<PresetColor>(PresetColor.blue);
  readonly themePrimaryColor$ = this.themePrimaryColor.asObservable();

  private readonly isDarkTheme = new BehaviorSubject<boolean>(true);
  readonly isDarkTheme$ = this.isDarkTheme.asObservable();

  async setThemeModeAsync(themeMode: ThemeMode) {
    await this.settingsService.themeMode.setAsync(themeMode);
    await this.themeModeGlobalSetter.setAsync(themeMode);
  }


  async setThemePrimaryColorAsync(color: PresetColor) {
    await this.settingsService.themePrimaryColor.setAsync(color);
    await this.themePrimaryColorGlobalSetter.setAsync(color);
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


  private onThemeColorGloballyChanged(e: Event<PresetColor>) {
    this.setPresetColor(e.payload);
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


  private setPresetColor(color: PresetColor) {
    const currentTheme = this.primeNg.theme();
    currentTheme.preset = getThemePreset(color);
    this.primeNg.onThemeChange(currentTheme);
    this.themePrimaryColor.next(color);
  }


  private notifyThemeChanged() {
    this.isDarkTheme.next(this.isDarkMode);
  }
}
