import { Injectable } from "@angular/core";
import { emit, Event, listen } from "@tauri-apps/api/event";
import { BehaviorSubject } from "rxjs";
import { SettingsService, Size } from "./settings.service";

const SIZE_CHANGED_EVENT_NAME = 'size-changed-event';
const PINNED_CLIPS_HEIGHT_CHANGED_EVENT_NAME = 'pinned-clips-height-changed-event';

@Injectable({ providedIn: 'root' })
export class PasteWindowSizingService {
  constructor(
    private readonly settingsService: SettingsService,
  ) {
    this.settingsService.getPasteWindowSizeAsync().then(size => {
      this.width.next(size.width);
      this.height.next(size.height);
    });
    this.settingsService.getPasteWindowPinnedClipsHeightPercentageAsync().then(percentage => {
      this.pinnedClipsHeightPercentage.next(percentage);
    });
    listen<Size>(SIZE_CHANGED_EVENT_NAME, this.onSizeGloballyChanged.bind(this));
    listen<number>(PINNED_CLIPS_HEIGHT_CHANGED_EVENT_NAME, this.onPinnedClipsHeightGloballyChanged.bind(this));
  }

  private readonly width = new BehaviorSubject<number>(0);
  readonly width$ = this.width.asObservable();

  private readonly height = new BehaviorSubject<number>(0);
  readonly height$ = this.height.asObservable();

  private readonly pinnedClipsHeightPercentage = new BehaviorSubject<number>(0);
  readonly pinnedClipsHeightPercentage$ = this.pinnedClipsHeightPercentage.asObservable();


  async setSize(width: number, height: number) {
    await this.settingsService.setPasteWindowSizeAsync({ width, height });
    await emit<Size>(SIZE_CHANGED_EVENT_NAME, { width, height });
  }

  async setPinnedClipsHeightPercentage(heightPercentage: number) {
    await this.settingsService.setPasteWindowPinnedClipsHeightPercentageAsync(heightPercentage);
    await emit<number>(PINNED_CLIPS_HEIGHT_CHANGED_EVENT_NAME, heightPercentage);
  }


  private onSizeGloballyChanged(e: Event<Size>) {
    this.width.next(e.payload.width);
    this.height.next(e.payload.height);
  }

  private onPinnedClipsHeightGloballyChanged(e: Event<number>) {
    this.pinnedClipsHeightPercentage.next(e.payload);
  }
}
