import { Injectable } from "@angular/core";
import { _, TranslateService } from "@ngx-translate/core";
import { defaultWindowIcon } from '@tauri-apps/api/app';
import { invoke } from "@tauri-apps/api/core";
import { Image } from "@tauri-apps/api/image";
import { CheckMenuItem, Menu, MenuItem, Submenu } from "@tauri-apps/api/menu";
import { TrayIcon } from '@tauri-apps/api/tray';
import { exit } from '@tauri-apps/plugin-process';
import { firstValueFrom } from "rxjs";
import { LANGUAGE_INFO } from "../../core/constants/language-info";
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

    const languageItems: CheckMenuItem[] = [];
    for (const lang of this.translateService.getLangs()) {
      const langItem = await CheckMenuItem.new({
        id: lang,
        text: LANGUAGE_INFO[lang].nativeName,
        action: () => this.settingsService.setLanguageAsync(lang)
      });
      languageItems.push(langItem);
    }
    this.languageMenuItem = await Submenu.new({
      text: 'Language',
      items: languageItems
    });

    this.exitMenuItem = await MenuItem.new({
      text: 'Exit',
      action: () => exit()
    });

    this.translateService.onLangChange.subscribe(() => this.updateTranslationsAsync());
    await this.updateTranslationsAsync();

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


  private async updateTranslationsAsync() {
    this.settingsMenuItem?.setText(await firstValueFrom(this.translateService.get(_('settings'))));
    this.aboutMenuItem?.setText(await firstValueFrom(this.translateService.get(_('about'))));
    this.languageMenuItem?.setText(await firstValueFrom(this.translateService.get(_('language'))));
    this.exitMenuItem?.setText(await firstValueFrom(this.translateService.get(_('exit'))));
    this.languageMenuItem?.items().then(items => {
      for (const item of items) {
        if (item instanceof CheckMenuItem) {
          const checked = item.id === this.translateService.currentLang;
          item.setChecked(checked);
          item.setEnabled(!checked);
        }
      }
    });
  }


  private async showMainWindowAsync() {
    await invoke('open_main_window', { section: 'settings' });
  }
}
