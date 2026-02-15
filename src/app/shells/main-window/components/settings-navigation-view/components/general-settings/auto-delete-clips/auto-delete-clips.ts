import { Component, effect, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ClipsAutoDeleteService } from '@app/core/services/clips-auto-delete.service';
import { DeletionPeriodType } from '@app/core/services/settings.service';
import { getPluralCategory } from '@app/core/utils/plural.utils';
import { SettingsCardComponent } from '@app/shells/main-window/components/settings-card/settings-card.component';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { InputNumber } from 'primeng/inputnumber';
import { Select } from 'primeng/select';

@Component({
  selector: 'jcm-auto-delete-clips',
  templateUrl: './auto-delete-clips.html',
  styleUrl: './auto-delete-clips.scss',
  imports: [
    SettingsCardComponent,
    FormsModule,
    TranslatePipe,
    FormsModule,
    InputNumber,
    Select,
  ],
})
export class AutoDeleteClips implements OnInit {
  private readonly clipsAutoDeleteService = inject(ClipsAutoDeleteService);
  private readonly translateService = inject(TranslateService);

  ngOnInit(): void {
    this.clipsAutoDeleteService.getClipsAutoDeletePeriodAsync().then(({ quantity, periodType }) => {
      this.periodQuantity.set(quantity);
      this.selectedDeletionPeriodType.set(periodType);
    });
  }

  protected readonly periodQuantity = signal(3);
  protected readonly selectedDeletionPeriodType = signal(DeletionPeriodType.Day);
  private readonly autoDeletePeriodEffect = effect(async () => {
    const periodQuantity = this.periodQuantity();
    const selectedDeletionPeriodType = this.selectedDeletionPeriodType();
    await this.clipsAutoDeleteService.setClipsAutoDeletePeriodAsync(periodQuantity, selectedDeletionPeriodType);
  });


  protected readonly deletionPeriodTypes = this.clipsAutoDeleteService.deletionPeriodTypes;


  protected getPluralCategory(quantity: number): Intl.LDMLPluralRule {
    return getPluralCategory(quantity, this.translateService.getCurrentLang());
  }
}
