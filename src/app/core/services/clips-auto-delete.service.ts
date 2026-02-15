import { Injectable } from "@angular/core";
import { ClipsRepository } from "../data/repositories/clips.repository";
import { DeletionPeriodType, SettingsService } from "./settings.service";

@Injectable({ providedIn: 'root' })
export class ClipsAutoDeleteService {
  constructor(
    private readonly settingsService: SettingsService,
    private readonly clipsRepository: ClipsRepository,
  ) { }

  readonly deletionPeriodTypes: DeletionPeriodType[] = Object.values(DeletionPeriodType);

  async getClipsAutoDeletePeriodAsync(): Promise<{ quantity: number, periodType: DeletionPeriodType; }> {
    return await this.settingsService.clipsAutoDeletePeriod.getAsync();
  }

  async setClipsAutoDeletePeriodAsync(periodQuantity: number, periodType: DeletionPeriodType) {
    await this.settingsService.clipsAutoDeletePeriod.setAsync({ quantity: periodQuantity, periodType: periodType });
  }

  async deleteOutdatedClipsAsync(): Promise<void> {
    const autoDeletePeriod = await this.settingsService.clipsAutoDeletePeriod.getAsync();
    if (autoDeletePeriod.quantity <= 0) {
      return;
    }

    const olderThan = new Date();
    switch (autoDeletePeriod.periodType) {
      case DeletionPeriodType.Day:
        olderThan.setDate(olderThan.getDate() - autoDeletePeriod.quantity);
        break;
      case DeletionPeriodType.Month:
        olderThan.setMonth(olderThan.getMonth() - autoDeletePeriod.quantity);
        break;
      case DeletionPeriodType.Year:
        olderThan.setFullYear(olderThan.getFullYear() - autoDeletePeriod.quantity);
        break;
    }

    await this.clipsRepository.deleteOutdatedClipsAsync(olderThan);
  }
}
