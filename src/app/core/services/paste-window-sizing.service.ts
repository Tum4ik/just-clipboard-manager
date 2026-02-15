import { Injectable } from "@angular/core";
import { Event } from "@tauri-apps/api/event";
import { BehaviorSubject } from "rxjs";
import { GlobalStateService } from "./base/global-state-service";
import { SettingsService, Size } from "./settings.service";

@Injectable({ providedIn: 'root' })
export class PasteWindowSizingService extends GlobalStateService {
  constructor(
    private readonly settingsService: SettingsService,
  ) {
    super();

    this.settingsService.pasteWindowSize.getAsync().then(size => {
      this.width.next(size.width);
      this.height.next(size.height);
    });
    this.settingsService.pasteWindowPinnedClipsHeightPercentage.getAsync().then(percentage => {
      this.pinnedClipsHeightPercentage.next(percentage);
    });
  }

  private readonly sizeGlobalSetter = this.registerGlobalObservable(
    'size-changed-event',
    this.onSizeGloballyChanged.bind(this)
  );
  private readonly pinnedClipsAreaHeightGlobalSetter = this.registerGlobalObservable(
    'pinned-clips-height-changed-event',
    this.onPinnedClipsHeightGloballyChanged.bind(this)
  );

  private readonly width = new BehaviorSubject<number>(0);
  readonly width$ = this.width.asObservable();

  private readonly height = new BehaviorSubject<number>(0);
  readonly height$ = this.height.asObservable();

  private readonly pinnedClipsHeightPercentage = new BehaviorSubject<number>(0);
  readonly pinnedClipsHeightPercentage$ = this.pinnedClipsHeightPercentage.asObservable();


  async setSize(width: number, height: number) {
    await this.settingsService.pasteWindowSize.setAsync({ width, height });
    await this.sizeGlobalSetter.setAsync({ width, height });
  }

  async setPinnedClipsHeightPercentage(heightPercentage: number) {
    await this.settingsService.pasteWindowPinnedClipsHeightPercentage.setAsync(heightPercentage);
    await this.pinnedClipsAreaHeightGlobalSetter.setAsync(heightPercentage);
  }


  private onSizeGloballyChanged(e: Event<Size>) {
    this.width.next(e.payload.width);
    this.height.next(e.payload.height);
  }

  private onPinnedClipsHeightGloballyChanged(e: Event<number>) {
    this.pinnedClipsHeightPercentage.next(e.payload);
  }
}
