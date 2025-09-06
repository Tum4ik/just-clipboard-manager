import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ClipsAutoDeleteService } from '@app/core/services/clips-auto-delete.service';
import { DeletionPeriodType } from '@app/core/services/settings.service';
import { getPluralCategory } from '@app/core/utils/plural.utils';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { disable, enable, isEnabled } from '@tauri-apps/plugin-autostart';
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
export class GeneralSettingsComponent {
  constructor(
    private readonly clipsAutoDeleteService: ClipsAutoDeleteService,
    private readonly translateService: TranslateService,
  ) {
    isEnabled().then(enabled => this.isAutoStartEnabled = enabled);
    this.clipsAutoDeleteService.getClipsAutoDeletePeriodAsync().then(({ quantity, periodType }) => {
      this.periodQuantity = quantity;
      this.selectedDeletionPeriodType = periodType;
    });
  }

  isAutoStartEnabled = false;

  periodQuantity = 1;
  selectedDeletionPeriodType = DeletionPeriodType.Day;

  get deletionPeriodTypes() {
    return this.clipsAutoDeleteService.deletionPeriodTypes;
  }


  setAutoStart(e: ToggleSwitchChangeEvent) {
    if (e.checked) {
      enable();
    }
    else {
      disable();
    }
  }


  setClipsAutoDeletePeriod(periodQuantity: number, periodType: DeletionPeriodType) {
    this.clipsAutoDeleteService.setClipsAutoDeletePeriodAsync(periodQuantity, periodType);
  }


  getPluralCategory(quantity: number): Intl.LDMLPluralRule {
    return getPluralCategory(quantity, this.translateService.getCurrentLang());
  }
}
