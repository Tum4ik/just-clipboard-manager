import { Injectable } from "@angular/core";
import { _, TranslateService } from "@ngx-translate/core";
import { defaultWindowIcon } from '@tauri-apps/api/app';
import { invoke } from "@tauri-apps/api/core";
import { Image } from "@tauri-apps/api/image";
import { CheckMenuItem, Menu, MenuItem, Submenu } from "@tauri-apps/api/menu";
import { TrayIcon } from '@tauri-apps/api/tray';
import { exit } from '@tauri-apps/plugin-process';
import { firstValueFrom } from "rxjs";
import { SettingsService } from "../../core/services/settings.service";

@Injectable()
export class AppTray {
  constructor(
    private readonly translateService: TranslateService,
    private readonly settingsService: SettingsService
  ) { }

  private settingsMenuItem?: MenuItem;
  private aboutMenuItem?: MenuItem;

  private languageMenuItem?: Submenu;

  private exitMenuItem?: MenuItem;

  async initAsync() {
    this.settingsMenuItem = await MenuItem.new({
      text: 'Settings',
      action: () => this.showMainWindowAsync()
    });
    this.aboutMenuItem = await MenuItem.new({
      text: 'About',
    });
    this.languageMenuItem = await Submenu.new({
      text: 'Language',
      items: [
        await CheckMenuItem.new({
          id: 'en',
          text: 'English (United States)',
          action: () => this.setLanguageAsync('en')
        }),
        await CheckMenuItem.new({
          id: 'uk',
          text: 'українська (Україна)',
          action: () => this.setLanguageAsync('uk')
        }),
      ]
    });
    this.exitMenuItem = await MenuItem.new({
      text: 'Exit',
      action: () => exit()
    });

    this.updateTranslations();

    const menu = await Menu.new({
      items: [
        this.settingsMenuItem,
        this.aboutMenuItem,
        { kind: 'Predefined', item: 'Separator' },
        this.languageMenuItem,
        { kind: 'Predefined', item: 'Separator' },
        this.exitMenuItem,
      ],
    });

    const appName = "Just Clipboard Manager";
    const tray = await TrayIcon.new({
      icon: await defaultWindowIcon() as Image,
      title: appName,
      tooltip: appName,
      menu: menu,
      showMenuOnLeftClick: false,
    });
  }


  private async setLanguageAsync(lang: string) {
    await firstValueFrom(this.translateService.use(lang));
    await this.settingsService.setLanguageAsync(lang);
    this.updateTranslations();
  }


  private updateTranslations() {
    this.translateService.get(_('settings')).subscribe(text => this.settingsMenuItem?.setText(text));
    this.translateService.get(_('about')).subscribe(text => this.aboutMenuItem?.setText(text));
    this.translateService.get(_('language')).subscribe(text => this.languageMenuItem?.setText(text));
    this.translateService.get(_('exit')).subscribe(text => this.exitMenuItem?.setText(text));
    this.languageMenuItem?.items().then(items => {
      for (const item of items) {
        if (item instanceof CheckMenuItem) {
          item.setChecked(item.id === this.translateService.currentLang);
        }
      }
    });
  }


  private async showMainWindowAsync() {
    await invoke('open_main_window');
  }
}
