import { Component, computed, effect, inject, linkedSignal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';
import { ClipsAutoDeleteService } from '@app/core/services/clips-auto-delete.service';
import { getPluralCategory } from '@app/core/utils/plural.utils';
import { SettingsCardComponent } from '@app/shells/main-window/components/settings-card/settings-card.component';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { InputNumber } from 'primeng/inputnumber';
import { Select } from 'primeng/select';
import { from } from 'rxjs';

@Component({
  selector: 'jcm-auto-delete-clips',
  templateUrl: './auto-delete-clips.html',
  styleUrl: './auto-delete-clips.scss',
  imports: [
    SettingsCardComponent,
    FormsModule,
    TranslatePipe,
    InputNumber,
    Select,
  ],
})
export class AutoDeleteClips {
  private readonly clipsAutoDeleteService = inject(ClipsAutoDeleteService);
  private readonly translateService = inject(TranslateService);

  private readonly autoDeletePeriod = toSignal(from(this.clipsAutoDeleteService.getClipsAutoDeletePeriodAsync()));
  protected readonly periodQuantity = linkedSignal(() => this.autoDeletePeriod()?.quantity);
  protected readonly selectedDeletionPeriodType = linkedSignal(() => this.autoDeletePeriod()?.periodType);
  private readonly autoDeletePeriodEffect = effect(async () => {
    const periodQuantity = this.periodQuantity();
    const selectedDeletionPeriodType = this.selectedDeletionPeriodType();
    if (periodQuantity && selectedDeletionPeriodType) {
      await this.clipsAutoDeleteService.setClipsAutoDeletePeriodAsync(periodQuantity, selectedDeletionPeriodType);
    }
  });


  protected readonly deletionPeriodTypes = this.clipsAutoDeleteService.deletionPeriodTypes;

  protected readonly pluralCategory = computed(() => {
    const quantity = this.periodQuantity();
    if (quantity || quantity === 0) {
      return getPluralCategory(quantity, this.translateService.getCurrentLang());
    }
    return undefined;
  });
}
