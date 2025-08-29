import { Injectable } from "@angular/core";
import { emit, Event, listen } from "@tauri-apps/api/event";
import { BehaviorSubject } from "rxjs";
import { SettingsService, Size } from "./settings.service";

const SIZE_CHANGED_EVENT_NAME = 'size-changed-event';

@Injectable({ providedIn: 'root' })
export class PasteWindowSizingService {
  constructor(
    private readonly settingsService: SettingsService,
  ) {
    this.settingsService.getPasteWindowSizeAsync().then(size => {
      this.width.next(size.width);
      this.height.next(size.height);
    });
    listen<Size>(SIZE_CHANGED_EVENT_NAME, this.onSizeGloballyChanged.bind(this));
  }

  private readonly width = new BehaviorSubject<number>(0);
  readonly width$ = this.width.asObservable();

  private readonly height = new BehaviorSubject<number>(0);
  readonly height$ = this.height.asObservable();


  async setSize(width: number, height: number) {
    await this.settingsService.setPasteWindowSizeAsync({ width, height });
    await emit<Size>(SIZE_CHANGED_EVENT_NAME, { width, height });
  }


  private onSizeGloballyChanged(e: Event<Size>) {
    this.width.next(e.payload.width);
    this.height.next(e.payload.height);
  }
}
