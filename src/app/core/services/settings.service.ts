import { Injectable } from '@angular/core';
import { LazyStore } from '@tauri-apps/plugin-store';

const LANGUAGE = 'language';
const THEME_MODE = 'theme-mode';
const THEME_PRESET = 'theme-preset';

@Injectable({ providedIn: 'root' })
export class SettingsService {
  constructor() {
    this.store = new LazyStore('settings.json', { autoSave: false });
  }

  private readonly store: LazyStore;


  async getLanguageAsync(): Promise<string> {
    return await this.store.get<string>(LANGUAGE) ?? 'en';
  }

  async setLanguageAsync(language: string): Promise<void> {
    await this.store.set(LANGUAGE, language);
    await this.store.save();
  }

  onLanguageChanged(cb: (value: string | undefined) => void): Promise<() => void> {
    return this.store.onChange<string>((k, v) => {
      if (k === LANGUAGE) {
        cb(v);
      }
    });
  }


  async getThemeModeAsync(): Promise<ThemeMode> {
    return await this.store.get<ThemeMode>(THEME_MODE) ?? 'system';
  }

  async setThemeModeAsync(themeMode: ThemeMode): Promise<void> {
    await this.store.set(THEME_MODE, themeMode);
    await this.store.save();
  }

  onThemeModeChanged(cb: (value: ThemeMode | undefined) => void): Promise<() => void> {
    return this.store.onChange<ThemeMode>((k, v) => {
      if (k === THEME_MODE) {
        cb(v);
      }
    });
  }
}


export type ThemeMode = 'system' | 'light' | 'dark';
