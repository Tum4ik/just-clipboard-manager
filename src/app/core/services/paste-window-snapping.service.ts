import { Injectable } from "@angular/core";
import { emit, Event, listen } from "@tauri-apps/api/event";
import { DisplayEdgePosition, SettingsService, SnappingMode } from "./settings.service";

const SNAPPING_MODE_CHANGED_EVENT_NAME = 'snapping-mode-changed-event';
const DISPLAY_EDGE_POSITION_CHANGED_EVENT_NAME = 'display-edge-position-changed-event';

@Injectable({ providedIn: 'root' })
export class PasteWindowSnappingService {
  constructor(
    private readonly settingsService: SettingsService,
  ) {
    listen<SnappingMode>(
      SNAPPING_MODE_CHANGED_EVENT_NAME,
      this.onSnappingModeGloballyChanged.bind(this)
    );
    listen<DisplayEdgePosition>(
      DISPLAY_EDGE_POSITION_CHANGED_EVENT_NAME,
      this.onDisplayEdgePositionGloballyChanged.bind(this)
    );
  }

  readonly snappingModes: SnappingMode[] = Object.values(SnappingMode);
  readonly displayEdgePositions: DisplayEdgePosition[] = Object.values(DisplayEdgePosition);


  private snappingMode?: SnappingMode;
  async getSnappingModeAsync(): Promise<SnappingMode> {
    return this.snappingMode ??= await this.settingsService.pasteWindowSnappingMode.getAsync();
  }

  async setSnappingModeAsync(mode: SnappingMode): Promise<void> {
    await this.settingsService.pasteWindowSnappingMode.setAsync(mode);
    await emit<SnappingMode>(SNAPPING_MODE_CHANGED_EVENT_NAME, mode);
  }


  private displayEdgePosition?: DisplayEdgePosition;
  async getDisplayEdgePositionAsync(): Promise<DisplayEdgePosition> {
    return this.displayEdgePosition ??= await this.settingsService.pasteWindowDisplayEdgePosition.getAsync();
  }

  async setDisplayEdgePositionAsync(position: DisplayEdgePosition): Promise<void> {
    await this.settingsService.pasteWindowDisplayEdgePosition.setAsync(position);
    await emit<DisplayEdgePosition>(DISPLAY_EDGE_POSITION_CHANGED_EVENT_NAME, position);
  }


  private onSnappingModeGloballyChanged(e: Event<SnappingMode>) {
    this.snappingMode = e.payload;
  }

  private onDisplayEdgePositionGloballyChanged(e: Event<DisplayEdgePosition>) {
    this.displayEdgePosition = e.payload;
  }
}
