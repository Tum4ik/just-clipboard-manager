import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatIcon } from '@angular/material/icon';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { Select, SelectChangeEvent } from 'primeng/select';
import { Subscription } from 'rxjs';
import { LANGUAGE_INFO } from '../../../../../../core/constants/language-info';
import { SettingsService, ThemeMode } from '../../../../../../core/services/settings.service';
import { SettingsCardComponent } from "../../../settings-card/settings-card.component";

@Component({
  selector: 'jcm-interface-settings',
  templateUrl: './interface-settings.component.html',
  styleUrl: './interface-settings.component.scss',
  imports: [
    SettingsCardComponent,
    TranslatePipe,
    Select,
    MatIcon,
    FormsModule
  ]
})
export class InterfaceSettingsComponent implements OnInit, OnDestroy {
  constructor(
    private readonly translateService: TranslateService,
    private readonly settingsService: SettingsService
  ) {
    this.languages = this.translateService.getLangs()
      .map(l => ({ code: l, nativeName: LANGUAGE_INFO[l].nativeName }) as LanguageItem);
  }

  private langChangedSubscription?: Subscription;

  readonly languages: LanguageItem[];
  selectedLanguage: LanguageItem | undefined;

  readonly themeModes: ThemeMode[] = ['system', 'light', 'dark'];
  selectedThemeMode: ThemeMode | undefined;

  async ngOnInit(): Promise<void> {
    this.selectedLanguage = this.languages.find(l => l.code === this.translateService.currentLang);
    this.langChangedSubscription = this.translateService.onLangChange.subscribe(e => {
      this.selectedLanguage = this.languages.find(l => l.code === e.lang);
    });
    this.selectedThemeMode = await this.settingsService.getThemeModeAsync();
  }

  ngOnDestroy(): void {
    this.langChangedSubscription?.unsubscribe();
  }

  onLanguageChanged(e: SelectChangeEvent) {
    if (this.selectedLanguage?.code) {
      this.settingsService.setLanguageAsync(this.selectedLanguage.code);
    }
  }

  onThemeModeChanged(e: SelectChangeEvent) {
    if (this.selectedThemeMode) {
      this.settingsService.setThemeModeAsync(this.selectedThemeMode);
    }
  }
}


export interface LanguageItem {
  code: string;
  nativeName: string;
}
