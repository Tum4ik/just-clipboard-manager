import { inject, Injectable } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { Event } from "@tauri-apps/api/event";
import { BehaviorSubject, firstValueFrom } from 'rxjs';
import { GlobalStateService } from './base/global-state-service';
import { Language, SettingsService } from './settings.service';

@Injectable({
  providedIn: 'root',
})
export class LanguageSwitchingService extends GlobalStateService {
  private readonly settingsService = inject(SettingsService);
  private readonly translateService = inject(TranslateService);

  constructor() {
    super();
    this.init();
  }

  private readonly languageGlobalSetter = this.registerGlobalObservable(
    'language-globally-changed-event',
    this.onLanguageGloballyChanged.bind(this)
  );

  private readonly currentLanguage = new BehaviorSubject<Language | undefined>(undefined);
  readonly currentLanguage$ = this.currentLanguage.asObservable();

  readonly supportedLanguages = Object.values(Language) as readonly Language[];


  async setLanguageAsync(lang: Language) {
    await this.languageGlobalSetter.setAsync(lang);
  }


  private async init() {
    const currLang = await this.settingsService.language.getAsync();
    await firstValueFrom(this.translateService.use(currLang));
    this.currentLanguage.next(currLang);
  }

  private async onLanguageGloballyChanged(e: Event<Language>) {
    const newLang = e.payload;
    await this.settingsService.language.setAsync(newLang);
    await firstValueFrom(this.translateService.use(newLang));
    this.currentLanguage.next(newLang);
  }
}
