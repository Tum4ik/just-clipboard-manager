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


  getLanguage(): string {
    return this.store.get('language') as string;
  }

  setLanguage(value: string): void {
    this.store.set('language', value);
  }

  onLanguageChanged(callback: (newValue: string) => void): Unsubscribe {
    return this.store.onDidChange('language', (newValue, oldValue) => callback(newValue as string));
  }
}


export type Unsubscribe = () => void;
