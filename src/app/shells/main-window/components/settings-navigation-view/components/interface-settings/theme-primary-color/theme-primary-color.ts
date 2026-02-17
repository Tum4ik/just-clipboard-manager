import { TitleCasePipe } from '@angular/common';
import { Component, computed, inject } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';
import { ThemeService } from '@app/core/services/theme.service';
import { SettingsCardComponent } from "@app/shells/main-window/components/settings-card/settings-card.component";
import { PRESET_COLORS_MAP, PresetColorName } from '@app/theming/get-theme-preset';
import { TranslatePipe } from '@ngx-translate/core';
import { Select } from 'primeng/select';

@Component({
  selector: 'jcm-theme-primary-color',
  templateUrl: './theme-primary-color.html',
  styleUrl: './theme-primary-color.scss',
  imports: [
    SettingsCardComponent,
    TranslatePipe,
    TitleCasePipe,
    Select,
    FormsModule,
  ],
})
export class ThemePrimaryColor {
  private readonly themeService = inject(ThemeService);

  protected readonly colors = Object.keys(PRESET_COLORS_MAP);

  private readonly _selectedColor = toSignal(this.themeService.themePrimaryColor$, { requireSync: true });
  protected readonly selectedColor = computed(() => this._selectedColor());


  protected async setSelectedColor(color: PresetColorName) {
    await this.themeService.setThemePrimaryColorAsync(color);
  }

  protected getColorHex(item: any): string {
    return PRESET_COLORS_MAP[item as PresetColorName];
  }
}
