import { TitleCasePipe } from '@angular/common';
import { Component, computed, inject } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';
import { ThemeService } from '@app/core/services/theme.service';
import { SettingsCardComponent } from "@app/shells/main-window/components/settings-card/settings-card.component";
import { PresetColor } from '@app/theming/get-theme-preset';
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

  protected readonly colors = Object.values(PresetColor).map(this.getPresetColorItem);

  private readonly _selectedColor = toSignal(this.themeService.themePrimaryColor$, { requireSync: true });
  protected readonly selectedColor = computed(() => this.getPresetColorItem(this._selectedColor()));


  protected async setSelectedColor(colorItem: PresetColorItem) {
    await this.themeService.setThemePrimaryColorAsync(colorItem.name);
  }


  private getPresetColorItem(color: PresetColor): PresetColorItem {
    switch (color) {
      case PresetColor.emerald:
        return { name: color, color: '#10b981' };
      case PresetColor.green:
        return { name: color, color: '#22c55e' };
      case PresetColor.lime:
        return { name: color, color: '#84cc16' };
      case PresetColor.red:
        return { name: color, color: '#ef4444' };
      case PresetColor.orange:
        return { name: color, color: '#f97316' };
      case PresetColor.amber:
        return { name: color, color: '#f59e0b' };
      case PresetColor.yellow:
        return { name: color, color: '#eab308' };
      case PresetColor.teal:
        return { name: color, color: '#14b8a6' };
      case PresetColor.cyan:
        return { name: color, color: '#06b6d4' };
      case PresetColor.sky:
        return { name: color, color: '#0ea5e9' };
      case PresetColor.blue:
        return { name: color, color: '#3b82f6' };
      case PresetColor.indigo:
        return { name: color, color: '#6366f1' };
      case PresetColor.violet:
        return { name: color, color: '#8b5cf6' };
      case PresetColor.purple:
        return { name: color, color: '#a855f7' };
      case PresetColor.fuchsia:
        return { name: color, color: '#d946ef' };
      case PresetColor.pink:
        return { name: color, color: '#ec4899' };
      case PresetColor.rose:
        return { name: color, color: '#f43f5e' };
      case PresetColor.slate:
        return { name: color, color: '#64748b' };
      case PresetColor.gray:
        return { name: color, color: '#6b7280' };
      case PresetColor.zinc:
        return { name: color, color: '#71717a' };
      case PresetColor.neutral:
        return { name: color, color: '#737373' };
      case PresetColor.stone:
        return { name: color, color: '#78716c' };
    }
  }
}

export interface PresetColorItem {
  name: PresetColor;
  color: string;
}
