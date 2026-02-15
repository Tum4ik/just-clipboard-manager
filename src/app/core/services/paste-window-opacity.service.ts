import { Injectable } from "@angular/core";
import { emit, Event, listen } from "@tauri-apps/api/event";
import { Subject } from "rxjs";
import { SettingsService } from "./settings.service";

const OPACITY_PERCENTAGE_CHANGED_EVENT_NAME = 'opacity-percentage-changed-event';

@Injectable({ providedIn: 'root' })
export class PasteWindowOpacityService {
  constructor(
    private readonly settingsService: SettingsService,
  ) {
    this.settingsService.pasteWindowOpacityPercentage.getAsync().then(opacity => {
      this.opacityPercentage.next(opacity);
    });
    listen<number>(OPACITY_PERCENTAGE_CHANGED_EVENT_NAME, this.onOpacityPercentageGloballyChanged.bind(this));
  }

  private readonly opacityPercentage = new Subject<number>();
  readonly opacityPercentage$ = this.opacityPercentage.asObservable();


  async setOpacityPercentageAsync(opacity: number): Promise<void> {
    await this.settingsService.pasteWindowOpacityPercentage.setAsync(opacity);
    await emit<number>(OPACITY_PERCENTAGE_CHANGED_EVENT_NAME, opacity);
  }


  private onOpacityPercentageGloballyChanged(e: Event<number>) {
    this.opacityPercentage.next(e.payload);
  }
}
