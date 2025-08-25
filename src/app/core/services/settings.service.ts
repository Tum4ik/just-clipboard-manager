import { Injectable } from '@angular/core';
import { PhysicalSize } from '@tauri-apps/api/dpi';
import { LazyStore } from '@tauri-apps/plugin-store';

const LANGUAGE = 'language';
const THEME_MODE = 'theme-mode';
const THEME_PRESET = 'theme-preset';
const PASTE_WINDOW_SIZE = 'paste-window-size';
const PASTE_WINDOW_PANEL_SIZES = 'paste-window-panel-sizes';
const PASTE_WINDOW_SNAPPING_MODE = 'paste-window-snapping-mode';
const PINNED_CLIPS_ORDER = 'pinned-clips-order';

@Injectable({ providedIn: 'root' })
export class SettingsService {
  constructor() {
    this.store = new LazyStore('settings.json', { defaults: {}, autoSave: false });
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


  async getPasteWindowSizeAsync(): Promise<PhysicalSize> {
    const size = await this.store.get<Size>(PASTE_WINDOW_SIZE);
    return new PhysicalSize(size?.width ?? 400, size?.height ?? 400);
  }

  async setPasteWindowSizeAsync(size: PhysicalSize): Promise<void> {
    await this.store.set(PASTE_WINDOW_SIZE, size);
    await this.store.save();
  }


  async getPasteWindowPanelSizesAsync(): Promise<number[]> {
    return await this.store.get<number[]>(PASTE_WINDOW_PANEL_SIZES) ?? [40, 60];
  }

  async setPasteWindowPanelSizesAsync(sizes: number[]): Promise<void> {
    await this.store.set(PASTE_WINDOW_PANEL_SIZES, sizes);
    await this.store.save();
  }


  async getPasteWindowSnappingModeAsync(): Promise<SnappingMode> {
    return await this.store.get<SnappingMode>(PASTE_WINDOW_SNAPPING_MODE) ?? SnappingMode.MouseCursor;
  }

  async setPasteWindowSnappingModeAsync(mode: SnappingMode): Promise<void> {
    await this.store.set(PASTE_WINDOW_SNAPPING_MODE, mode);
    await this.store.save();
  }


  async getPinnedClipsOrderAsync(): Promise<number[]> {
    return await this.store.get<number[]>(PINNED_CLIPS_ORDER) ?? [];
  }

  async setPinnedClipsOrderAsync(order: number[]): Promise<void> {
    await this.store.set(PINNED_CLIPS_ORDER, order);
    await this.store.save();
  }
}


export type ThemeMode = 'system' | 'light' | 'dark';
export interface Size { width: number; height: number; }

export enum SnappingMode {
  MouseCursor = 'mouse-cursor',
  Caret = 'caret',
  DisplayEdges = 'display-edges'
}
