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

  protected readonly colors = Object.values(PresetColor);

  private readonly _selectedColor = toSignal(this.themeService.themePrimaryColor$, { requireSync: true });
  protected readonly selectedColor = computed(() => this._selectedColor());


  protected async setSelectedColor(color: PresetColor) {
    await this.themeService.setThemePrimaryColorAsync(color);
  }

  protected readonly colorMap: Readonly<Record<PresetColor, string>> = {
    [PresetColor.emerald]: '#10b981',
    [PresetColor.green]: '#22c55e',
    [PresetColor.lime]: '#84cc16',
    [PresetColor.red]: '#ef4444',
    [PresetColor.orange]: '#f97316',
    [PresetColor.amber]: '#f59e0b',
    [PresetColor.yellow]: '#eab308',
    [PresetColor.teal]: '#14b8a6',
    [PresetColor.cyan]: '#06b6d4',
    [PresetColor.sky]: '#0ea5e9',
    [PresetColor.blue]: '#3b82f6',
    [PresetColor.indigo]: '#6366f1',
    [PresetColor.violet]: '#8b5cf6',
    [PresetColor.purple]: '#a855f7',
    [PresetColor.fuchsia]: '#d946ef',
    [PresetColor.pink]: '#ec4899',
    [PresetColor.rose]: '#f43f5e',
    [PresetColor.slate]: '#64748b',
    [PresetColor.gray]: '#6b7280',
    [PresetColor.zinc]: '#71717a',
    [PresetColor.neutral]: '#737373',
    [PresetColor.stone]: '#78716c',
  };

  protected asPresetColor(item: any): PresetColor {
    return item as PresetColor;
  }
}

export interface PresetColorItem {
  name: PresetColor;
  color: string;
}
