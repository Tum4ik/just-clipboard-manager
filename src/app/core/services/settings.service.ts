import { Injectable } from '@angular/core';
import { LazyStore } from '@tauri-apps/plugin-store';

const LANGUAGE = 'language';

@Injectable({ providedIn: 'root' })
export class SettingsService {
  constructor() {
    this.store = new LazyStore('settings.json', { autoSave: false });
  }

  private readonly store: LazyStore;


  private language?: string;

  async getLanguageAsync(): Promise<string> {
    if (!this.language) {
      let language = await this.store.get<string>(LANGUAGE);
      if (!language) {
        language = 'en';
        await this.saveLanguageAsync(language);
      }
      this.language = language;
    }

    return this.language;
  }

  async setLanguageAsync(language: string): Promise<void> {
    if (this.language === language) {
      return;
    }

    this.language = language;
    await this.saveLanguageAsync(language);
  }


  private async saveLanguageAsync(language: string): Promise<void> {
    await this.store.set(LANGUAGE, language);
    await this.store.save();
  }
}
