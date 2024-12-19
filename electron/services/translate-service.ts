import i18n from 'i18next';
import FsBackend, { FsBackendOptions } from 'i18next-fs-backend';
import { inject, injectable } from "inversify";
import path from 'path';
import { TYPES } from "../ioc/types";
import { SettingsService } from './settings-service';

@injectable()
export class TranslateService {
  constructor(
    @inject(TYPES.AppDir) private readonly appDir: string,
    private readonly settings: SettingsService,
  ) { }


  private isInitialize = false;


  get selectedLanguage(): string {
    return i18n.language;
  }


  async initAsync(): Promise<void> {
    if (this.isInitialize) {
      return;
    }

    this.isInitialize = true;

    const lang = this.settings.language;
    await i18n.use(FsBackend)
      .init<FsBackendOptions>({
        backend: {
          loadPath: path.join(this.appDir, 'i18n/{{lng}}/{{ns}}.yml'),
          addPath: path.join(this.appDir, 'i18n/{{lng}}/{{ns}}.missing.yml'),
        },
        initImmediate: false,
        lng: lang, // todo: use selected lang from settings
        fallbackLng: 'en',
        preload: [lang], // todo: preload selected lang from settings
        ns: 'translation',
        defaultNS: 'translation'
      });
  }


  async changeLanguageAsync(lang: string): Promise<void> {
    await i18n.changeLanguage(lang);
  }


  translate(key: string): string {
    return i18n.t(key);
  }
}
