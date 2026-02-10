import { Component, effect, OnDestroy, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatIcon } from '@angular/material/icon';
import { GoogleIcon } from "@app/core/components/google-icon/google-icon";
import { SettingsService, ThemeMode } from '@app/core/services/settings.service';
import { ThemeService } from '@app/core/services/theme.service';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { Select } from 'primeng/select';
import { firstValueFrom, Subscription } from 'rxjs';
import { LANGUAGE_INFO } from '../../../../../../core/constants/language-info';
import { ScrollViewComponent } from "../../../scroll-view/scroll-view.component";
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
    FormsModule,
    ScrollViewComponent,
    GoogleIcon
  ]
})
export class InterfaceSettingsComponent implements OnInit, OnDestroy {
  constructor(
    private readonly translateService: TranslateService,
    private readonly settingsService: SettingsService,
    private readonly themeService: ThemeService
  ) {
    this.languages = this.translateService.getLangs()
      .map(l => ({ code: l, nativeName: LANGUAGE_INFO[l].nativeName }) as LanguageItem);
  }

  private langChangedSubscription?: Subscription;

  protected readonly languages: LanguageItem[];
  protected readonly selectedLanguage = signal<LanguageItem | undefined>(undefined);
  private readonly selectedLanguageEffect = effect(() => {
    const selectedLanguage = this.selectedLanguage();
    if (selectedLanguage?.code) {
      this.settingsService.setLanguageAsync(selectedLanguage.code);
    }
  });

  protected readonly themeModes: ThemeMode[] = ['system', 'light', 'dark'];
  protected readonly selectedThemeMode = signal<ThemeMode | undefined>(undefined);
  private readonly selectedThemeModeEffect = effect(() => {
    const selectedThemeMode = this.selectedThemeMode();
    if (selectedThemeMode) {
      this.themeService.setThemeModeAsync(selectedThemeMode);
    }
  });
  protected getThemeModeIconName(themeMode: ThemeMode): string {
    switch (themeMode) {
      case 'light': return 'light_mode';
      case 'dark': return 'dark_mode';
      default: return 'computer';
    }
  }

  async ngOnInit(): Promise<void> {
    this.setSelectedLanguage(this.translateService.getCurrentLang());
    this.langChangedSubscription = this.translateService.onLangChange.subscribe(e => {
      this.setSelectedLanguage(e.lang);
    });

    const selectedThemeMode = await firstValueFrom(this.themeService.themeMode$);
    this.selectedThemeMode.set(selectedThemeMode);
  }

  ngOnDestroy(): void {
    this.langChangedSubscription?.unsubscribe();
  }


  private setSelectedLanguage(lang: string) {
    const selectedLanguage = this.languages.find(l => l.code === lang);
    this.selectedLanguage.set(selectedLanguage);
  }
}


export interface LanguageItem {
  code: string;
  nativeName: string;
}
