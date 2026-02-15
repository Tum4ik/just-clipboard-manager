import { LazyStore } from '@tauri-apps/plugin-store';

export abstract class BaseSettingsService {
  protected constructor(fileName: string) {
    this.store = new LazyStore(`${fileName}.json`, { defaults: {}, autoSave: false });
  }

  protected readonly store: LazyStore;

  protected setting<T>(name: string, defaultValue: T): Setting<T> {
    return new Setting<T>(this.store, name, defaultValue);
  }
}

class Setting<T> {
  constructor(
    private readonly store: LazyStore,
    private readonly name: string,
    private readonly defaultValue: T
  ) { }

  async getAsync(): Promise<T> {
    return await this.store.get<T>(this.name) ?? this.defaultValue;
  }

  async setAsync(value: T): Promise<void> {
    await this.store.set(this.name, value);
    await this.store.save();
  }
}
