import { Component, effect, inject, linkedSignal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommandsService } from '@app/services/commands-service';
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
  ],
})
export class InterfaceSettings {
  private readonly translate = inject(TranslateService);
  private readonly commandsService = inject(CommandsService);

  protected readonly languages = this.translate.getLangs()
    .map(l => ({ label: this.translate.instant(`supported-languages.${l}`) as string, value: l }));

  // private readonly languageFromSettings = toSignal(from(this.commandsService.getSelectedLanguageAsync()));
  // protected readonly selectedLanguage = linkedSignal(this.languageFromSettings);
  protected readonly selectedLanguage = linkedSignal(this.translate.currentLang);
  private readonly selectedLanguageEffect = effect(async () => {
    const newLang = this.selectedLanguage();
    if (!newLang) {
      return;
    }
    await this.commandsService.setSelectedLanguageAsync(newLang);
  });
}
