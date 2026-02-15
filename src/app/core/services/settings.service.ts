import { Injectable } from '@angular/core';
import { Theme } from '@tauri-apps/api/window';
import { BaseSettingsService } from './base/base-settings-service';

@Injectable({ providedIn: 'root' })
export class SettingsService extends BaseSettingsService {
  constructor() {
    super('settings');
  }

  readonly language = this.setting<Language>('language', Language.en);
  readonly themeMode = this.setting<ThemeMode>('theme-mode', 'system');
  readonly pasteWindowSize = this.setting<Size>('paste-window-size', { width: 400, height: 400 });
  readonly pasteWindowPinnedClipsHeightPercentage = this.setting<number>('paste-window-height-percentage', 40);
  readonly pasteWindowSnappingMode = this.setting<SnappingMode>('paste-window-snapping-mode', SnappingMode.MouseCursor);
  readonly pasteWindowDisplayEdgePosition = this.setting<DisplayEdgePosition>(
    'paste-window-display-edge-position', DisplayEdgePosition.TopLeft
  );
  readonly pasteWindowOpacityPercentage = this.setting<number>('paste-window-opacity-percentage', 100);
  readonly pinnedClipsOrder = this.setting<number[]>('pinned-clips-order', []);
  readonly clipsAutoDeletePeriod = this.setting<AutoDeletePeriod>(
    'clips-auto-delete-period', { quantity: 3, periodType: DeletionPeriodType.Month }
  );
}


export enum Language {
  en = 'en',
  uk = 'uk',
}

export type ThemeMode = 'system' | Theme;
export interface Size { width: number; height: number; }
export interface AutoDeletePeriod { quantity: number; periodType: DeletionPeriodType; }

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

export enum DeletionPeriodType {
  Day = 'day',
  Month = 'month',
  Year = 'year',
}
