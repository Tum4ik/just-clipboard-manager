import { Injectable } from "@angular/core";
import { EnvironmentService } from "@app/core/services/environment.service";
import { MonitoringService } from "@app/core/services/monitoring.service";
import { TranslateService } from "@ngx-translate/core";
import { defaultWindowIcon } from '@tauri-apps/api/app';
import { invoke } from "@tauri-apps/api/core";
import { Image } from "@tauri-apps/api/image";
import { CheckMenuItem, Menu, MenuItem, Submenu } from "@tauri-apps/api/menu";
import { TrayIcon } from '@tauri-apps/api/tray';
import { exit, relaunch } from '@tauri-apps/plugin-process';
import { check, Update } from '@tauri-apps/plugin-updater';
import { firstValueFrom } from "rxjs";
import { LANGUAGE_INFO } from "../../core/constants/language-info";
import { SettingsService } from "../../core/services/settings.service";
import { MainWindowTabId } from "../main-window/main-window.component";

@Injectable()
export class AppTray {
  constructor(
    private readonly translateService: TranslateService,
    private readonly settingsService: SettingsService,
    private readonly monitoring: MonitoringService,
    private readonly environment: EnvironmentService,
  ) { }

  private settingsMenuItem?: MenuItem;
  private aboutMenuItem?: MenuItem;

  private languageMenuItem?: Submenu;

  private exitMenuItem?: MenuItem;

  async initAsync() {
    // todo: establish update service
    let update: Update | null = null;
    try {
      update = await check();
    } catch (error) {
      this.monitoring.error("Can't check updates.", error);
    }
    if (update && await this.environment.isProductionAsync()) {
      try {
        await update.downloadAndInstall();
        await relaunch();
        return;
      } catch (error) {
        this.monitoring.error("Can't download and install.", error);
      }
    }


    this.settingsMenuItem = await MenuItem.new({
      text: 'Settings',
      action: () => this.showMainWindowAsync(MainWindowTabId.settings)
    });
    this.aboutMenuItem = await MenuItem.new({
      text: 'About',
      action: () => this.showMainWindowAsync(MainWindowTabId.about)
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

    const productName = await invoke<string | null>('info_product_name');
    const tray = await TrayIcon.new({
      icon: await defaultWindowIcon() as Image,
      menu: menu,
      showMenuOnLeftClick: false,
    });
    if (productName) {
      tray.setTitle(productName);
      tray.setTooltip(productName);
    }
  }


  private async updateTranslationsAsync() {
    this.settingsMenuItem?.setText(await firstValueFrom(this.translateService.get('settings')));
    this.aboutMenuItem?.setText(await firstValueFrom(this.translateService.get('about')));
    this.languageMenuItem?.setText(await firstValueFrom(this.translateService.get('language')));
    this.exitMenuItem?.setText(await firstValueFrom(this.translateService.get('exit')));
    this.languageMenuItem?.items().then(items => {
      for (const item of items) {
        if (item instanceof CheckMenuItem) {
          const checked = item.id === this.translateService.getCurrentLang();
          item.setChecked(checked);
          item.setEnabled(!checked);
        }
      }
    });
  }


  private async showMainWindowAsync(topLevelTabId: MainWindowTabId) {
    await invoke('open_main_window', { topLevelTabId: topLevelTabId });
  }
}
