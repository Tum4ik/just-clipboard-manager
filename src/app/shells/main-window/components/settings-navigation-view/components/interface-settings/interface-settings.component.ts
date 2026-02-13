import { Component, effect, inject, linkedSignal, OnDestroy, OnInit, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';
import { MatIcon } from '@angular/material/icon';
import { GoogleIcon } from "@app/core/components/google-icon/google-icon";
import { LanguageSwitchingService } from '@app/core/services/language-switching-service';
import { Language, ThemeMode } from '@app/core/services/settings.service';
import { ThemeService } from '@app/core/services/theme.service';
import { TranslatePipe } from '@ngx-translate/core';
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

    private readonly themeService: ThemeService
  ) {
    this.languages = this.languageSwitchingService.supportedLanguages
      .map(l => ({ code: l, nativeName: LANGUAGE_INFO[l].nativeName }) as LanguageItem);
  }
  private readonly languageSwitchingService = inject(LanguageSwitchingService);

  private langChangedSubscription?: Subscription;

  protected readonly languages: LanguageItem[];

  private readonly currentLanguage = toSignal(this.languageSwitchingService.currentLanguage$);
  protected readonly selectedLanguage = linkedSignal(() => {
    const currentLanguage = this.currentLanguage();
    return this.languages.find(l => l.code === currentLanguage);
  });
  private readonly selectedLanguageEffect = effect(async () => {
    const selectedLanguage = this.selectedLanguage();
    if (selectedLanguage?.code) {
      await this.languageSwitchingService.setLanguageAsync(Language[selectedLanguage.code as keyof typeof Language]);
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
    const selectedThemeMode = await firstValueFrom(this.themeService.themeMode$);
    this.selectedThemeMode.set(selectedThemeMode);
  }

  ngOnDestroy(): void {
    this.langChangedSubscription?.unsubscribe();
  }
}


export interface LanguageItem {
  code: string;
  nativeName: string;
}
