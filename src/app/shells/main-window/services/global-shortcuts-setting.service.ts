import { Injectable } from "@angular/core";
import { BaseShortcutsService } from "@app/core/services/base-shortcuts.service";
import { MonitoringService } from "@app/core/services/monitoring.service";

@Injectable()
export class GlobalShortcutsSettingService extends BaseShortcutsService {
  constructor(
    protected override readonly monitoring: MonitoringService,
  ) {
    super(monitoring);
  }
}
