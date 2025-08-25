import { Injectable } from "@angular/core";
import { emit, Event, listen } from "@tauri-apps/api/event";
import { SettingsService, SnappingMode } from "./settings.service";

const SNAPPING_MODE_CHANGED_EVENT_NAME = 'snapping-mode-changed-event';

@Injectable({ providedIn: 'root' })
export class PasteWindowSnappingService {
  constructor(
    private readonly settingsService: SettingsService,
  ) {
    listen<SnappingMode>(SNAPPING_MODE_CHANGED_EVENT_NAME, this.onSnappingModeGloballyChanged.bind(this));
  }

  readonly snappingModes: SnappingMode[] = Object.values(SnappingMode);

  private snappingMode?: SnappingMode;
  async getSnappingModeAsync(): Promise<SnappingMode> {
    return this.snappingMode ??= await this.settingsService.getPasteWindowSnappingModeAsync();
  }

  async setSnappingModeAsync(mode: SnappingMode): Promise<void> {
    await this.settingsService.setPasteWindowSnappingModeAsync(mode);
    await emit<SnappingMode>(SNAPPING_MODE_CHANGED_EVENT_NAME, mode);
  }


  private onSnappingModeGloballyChanged(e: Event<SnappingMode>) {
    this.snappingMode = e.payload;
  }
}
