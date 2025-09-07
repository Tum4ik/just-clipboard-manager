import { Injectable } from "@angular/core";
import { ClipsRepository } from "../data/repositories/clips.repository";
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

  async deleteOutdatedClips(): Promise<void> {
    const autoDeletePeriod = await this.settingsService.getClipsAutoDeletePeriodAsync();

    const olderThan = new Date();
    if (autoDeletePeriod.periodType === DeletionPeriodType.Day) {
      olderThan.setDate(olderThan.getDate() - autoDeletePeriod.quantity);
    } else if (autoDeletePeriod.periodType === DeletionPeriodType.Month) {
      olderThan.setMonth(olderThan.getMonth() - autoDeletePeriod.quantity);
    } else if (autoDeletePeriod.periodType === DeletionPeriodType.Year) {
      olderThan.setFullYear(olderThan.getFullYear() - autoDeletePeriod.quantity);
    }

    const clipsRepository = new ClipsRepository();
    await clipsRepository.deleteOutdatedClipsAsync(olderThan);
  }
}
