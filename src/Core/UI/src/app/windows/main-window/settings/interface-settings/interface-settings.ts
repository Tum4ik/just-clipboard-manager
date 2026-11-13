import { Component, effect, inject, linkedSignal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommandsService } from '@app/services/commands-service';
import { ThemeMode, ThemeService } from '@app/services/theme-service';
import { MaterialIcon } from '@app/shared/material-icon/material-icon';
import { TranslateBlockDirective, TranslateService } from '@ngx-translate/core';
import { Select } from 'primeng/select';
import { SettingCard } from '../setting-card/setting-card';

@Component({
  selector: 'jcm-interface-settings',
  templateUrl: './interface-settings.html',
  styleUrl: './interface-settings.css',
  imports: [
    SettingCard,
    Select,
    TranslateBlockDirective,
    FormsModule,
    MaterialIcon
  ],
})
export class InterfaceSettings {
  private readonly translate = inject(TranslateService);
  private readonly commandsService = inject(CommandsService);
  private readonly themeService = inject(ThemeService);


  protected readonly languages = this.translate.getLangs()
    .map(l => ({ label: this.translate.instant(`supported-languages.${l}`) as string, value: l }));

  protected readonly selectedLanguage = linkedSignal(this.translate.currentLang);
  private readonly selectedLanguageEffect = effect(async () => {
    const newLang = this.selectedLanguage();
    if (!newLang) {
      return;
    }
    await this.commandsService.setSelectedLanguageAsync(newLang);
  });


  protected readonly themeModes = [...this.themeService.themeModes];
  protected getThemeModeIconName(themeMode: ThemeMode): string {
    switch (themeMode) {
      case 'light': return 'light_mode';
      case 'dark': return 'dark_mode';
      default: return 'computer';
    }
  }
}
