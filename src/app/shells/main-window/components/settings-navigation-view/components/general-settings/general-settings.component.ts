import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ClipsAutoDeleteService } from '@app/core/services/clips-auto-delete.service';
import { DeletionPeriodType } from '@app/core/services/settings.service';
import { getPluralCategory } from '@app/core/utils/plural.utils';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { invoke } from '@tauri-apps/api/core';
import { InputNumber } from 'primeng/inputnumber';
import { Select } from "primeng/select";
import { ToggleSwitch, ToggleSwitchChangeEvent } from 'primeng/toggleswitch';
import { ScrollViewComponent } from "../../../scroll-view/scroll-view.component";
import { SettingsCardComponent } from "../../../settings-card/settings-card.component";

@Component({
  selector: 'jcm-general-settings',
  templateUrl: './general-settings.component.html',
  styleUrl: './general-settings.component.scss',
  imports: [
    TranslatePipe,
    ScrollViewComponent,
    SettingsCardComponent,
    ToggleSwitch,
    FormsModule,
    InputNumber,
    Select,
  ]
})
export class GeneralSettingsComponent implements OnInit {
  constructor(
    private readonly clipsAutoDeleteService: ClipsAutoDeleteService,
    private readonly translateService: TranslateService,
  ) { }

  async ngOnInit(): Promise<void> {
    this.isAutoStartEnabled = await invoke('autostart_is_enabled');
    const { quantity, periodType } = await this.clipsAutoDeleteService.getClipsAutoDeletePeriodAsync();
    this.periodQuantity = quantity;
    this.selectedDeletionPeriodType = periodType;
  }

  isAutoStartEnabled = false;

  periodQuantity = 1;
  selectedDeletionPeriodType = DeletionPeriodType.Day;

  get deletionPeriodTypes() {
    return this.clipsAutoDeleteService.deletionPeriodTypes;
  }


  async setAutoStart(e: ToggleSwitchChangeEvent) {
    if (e.checked) {
      await invoke('autostart_enable');
    }
    else {
      await invoke('autostart_disable');
    }
  }


  async setClipsAutoDeletePeriod(periodQuantity: number, periodType: DeletionPeriodType) {
    await this.clipsAutoDeleteService.setClipsAutoDeletePeriodAsync(periodQuantity, periodType);
  }


  getPluralCategory(quantity: number): Intl.LDMLPluralRule {
    return getPluralCategory(quantity, this.translateService.getCurrentLang());
  }
}
