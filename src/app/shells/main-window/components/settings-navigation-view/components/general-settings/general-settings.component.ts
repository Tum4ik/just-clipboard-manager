import { Component, effect, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ClipsAutoDeleteService } from '@app/core/services/clips-auto-delete.service';
import { DeletionPeriodType } from '@app/core/services/settings.service';
import { getPluralCategory } from '@app/core/utils/plural.utils';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { invoke } from '@tauri-apps/api/core';
import { InputNumber } from 'primeng/inputnumber';
import { Select } from "primeng/select";
import { ToggleSwitch } from 'primeng/toggleswitch';
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
    invoke<boolean>('autostart_is_enabled').then(enabled => this.isAutoStartEnabled.set(enabled));
    this.clipsAutoDeleteService.getClipsAutoDeletePeriodAsync().then(({ quantity, periodType }) => {
      this.periodQuantity.set(quantity);
      this.selectedDeletionPeriodType.set(periodType);
    });
  }

  protected readonly isAutoStartEnabled = signal(false);
  private readonly isAutoStartEnabledEffect = effect(async () => {
    const checked = this.isAutoStartEnabled();
    if (checked) {
      await invoke('autostart_enable');
    }
    else {
      await invoke('autostart_disable');
    }
  });

  protected readonly periodQuantity = signal(3);
  protected readonly selectedDeletionPeriodType = signal(DeletionPeriodType.Day);
  private readonly autoDeletePeriodEffect = effect(async () => {
    const periodQuantity = this.periodQuantity();
    const selectedDeletionPeriodType = this.selectedDeletionPeriodType();
    await this.clipsAutoDeleteService.setClipsAutoDeletePeriodAsync(periodQuantity, selectedDeletionPeriodType);
  });


  protected get deletionPeriodTypes() {
    return this.clipsAutoDeleteService.deletionPeriodTypes;
  }


  protected getPluralCategory(quantity: number): Intl.LDMLPluralRule {
    return getPluralCategory(quantity, this.translateService.getCurrentLang());
  }
}
