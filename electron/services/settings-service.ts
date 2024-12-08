import { injectable } from "inversify";
import 'reflect-metadata';

const Store = import('electron-store')
  .then(Store => {
    return new Store.default({
      name: 'settings',
      schema: {
        language: {
          type: 'string',
          default: 'en'
        }
      }
    });
  });


@injectable()
export class SettingsService {
  private store!: Awaited<typeof Store>;

  async initAsync() {
    if (this.store) {
      return;
    }
    this.store = await Store;
  }


  private _language?: string;
  get language(): string {
    if (this._language) {
      return this._language;
    }
    this._language = this.store.get('language') as string;
    return this._language;
  }
  set language(value: string) {
    if (this._language !== value) {
      this._language = value;
      this.store.set('language', value);
    }
  }

  onLanguageChanged(callback: (newValue: string) => void): Unsubscribe {
    return this.store.onDidChange('language', (newValue, oldValue) => callback(newValue as string));
  }
}


export type Unsubscribe = () => void;
