import { Injectable } from '@angular/core';
import { Theme } from '@tauri-apps/api/window';
import { LazyStore } from '@tauri-apps/plugin-store';

const LANGUAGE = 'language';
const THEME_MODE = 'theme-mode';
const THEME_PRESET = 'theme-preset';
const PASTE_WINDOW_SIZE = 'paste-window-size';
const PASTE_WINDOW_HEIGHT_PERCENTAGE = 'paste-window-height-percentage';
const PASTE_WINDOW_SNAPPING_MODE = 'paste-window-snapping-mode';
const PASTE_WINDOW_DISPLAY_EDGE_POSITION = 'paste-window-display-edge-position';
const PASTE_WINDOW_OPACITY_PERCENTAGE = 'paste-window-opacity-percentage';
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


  async getPasteWindowSizeAsync(): Promise<Size> {
    return await this.store.get<Size>(PASTE_WINDOW_SIZE) ?? { width: 400, height: 400 };
  }

  async setPasteWindowSizeAsync(size: Size): Promise<void> {
    await this.store.set(PASTE_WINDOW_SIZE, size);
    await this.store.save();
  }


  async getPasteWindowPinnedClipsHeightPercentageAsync(): Promise<number> {
    return await this.store.get<number>(PASTE_WINDOW_HEIGHT_PERCENTAGE) ?? 40;
  }

  async setPasteWindowPinnedClipsHeightPercentageAsync(heightPercentage: number): Promise<void> {
    await this.store.set(PASTE_WINDOW_HEIGHT_PERCENTAGE, heightPercentage);
    await this.store.save();
  }


  async getPasteWindowSnappingModeAsync(): Promise<SnappingMode> {
    return await this.store.get<SnappingMode>(PASTE_WINDOW_SNAPPING_MODE) ?? SnappingMode.MouseCursor;
  }

  async setPasteWindowSnappingModeAsync(mode: SnappingMode): Promise<void> {
    await this.store.set(PASTE_WINDOW_SNAPPING_MODE, mode);
    await this.store.save();
  }


  async getPasteWindowDisplayEdgePositionAsync(): Promise<DisplayEdgePosition> {
    return await this.store.get<DisplayEdgePosition>(PASTE_WINDOW_DISPLAY_EDGE_POSITION) ?? DisplayEdgePosition.TopLeft;
  }

  async setPasteWindowDisplayEdgePositionAsync(position: DisplayEdgePosition): Promise<void> {
    await this.store.set(PASTE_WINDOW_DISPLAY_EDGE_POSITION, position);
    await this.store.save();
  }


  async getPasteWindowOpacityPercentageAsync(): Promise<number> {
    return await this.store.get<number>(PASTE_WINDOW_OPACITY_PERCENTAGE) ?? 100;
  }

  async setPasteWindowOpacityPercentageAsync(opacity: number): Promise<void> {
    await this.store.set(PASTE_WINDOW_OPACITY_PERCENTAGE, opacity);
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


export type ThemeMode = 'system' | Theme;
export interface Size { width: number; height: number; }

export enum SnappingMode {
  MouseCursor = 'mouse-cursor',
  Caret = 'caret',
  DisplayEdges = 'display-edges'
}

export enum DisplayEdgePosition {
  TopLeft = 'top-left',
  TopRight = 'top-right',
  BottomLeft = 'bottom-left',
  BottomRight = 'bottom-right',
  TopCenter = 'top-center',
  BottomCenter = 'bottom-center',
  LeftCenter = 'left-center',
  RightCenter = 'right-center',
}
