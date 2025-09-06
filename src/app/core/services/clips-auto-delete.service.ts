import { Injectable } from "@angular/core";
import { DeletionPeriodType, SettingsService } from "./settings.service";

@Injectable({ providedIn: 'root' })
export class ClipsAutoDeleteService {
  constructor(
    private readonly settingsService: SettingsService,
  ) { }

  readonly deletionPeriodTypes: DeletionPeriodType[] = Object.values(DeletionPeriodType);

  async getClipsAutoDeletePeriodAsync(): Promise<{ quantity: number, periodType: DeletionPeriodType; }> {
    return await this.settingsService.getClipsAutoDeletePeriodAsync();
  }

  async setClipsAutoDeletePeriodAsync(periodQuantity: number, periodType: DeletionPeriodType) {
    await this.settingsService.setClipsAutoDeletePeriodAsync({ quantity: periodQuantity, periodType: periodType });
  }
}
