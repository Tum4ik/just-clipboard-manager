import { Injectable } from '@angular/core';
import { LazyStore } from '@tauri-apps/plugin-store';

const LANGUAGE = 'language';

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
}
