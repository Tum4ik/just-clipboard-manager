import { type App, BrowserWindow, globalShortcut, Menu, MenuItem, NativeImage, nativeImage, Tray } from "electron";
import { inject, injectable } from "inversify";
import path from 'path';
import { TYPES } from "../ioc/types";
import { SettingsService } from "../services/settings-service";
import { TranslateService } from "../services/translate-service";

@injectable()
export class AppTray {

  private readonly trayIcon: NativeImage;
  private readonly tray: Tray;
  private readonly isServe: boolean;

  constructor(
    @inject(TYPES.App) private readonly app: App,
    @inject(TYPES.AppDir) private readonly appDir: string,
    private readonly settings: SettingsService,
    private readonly i18n: TranslateService,
  ) {
    this.isServe = process.argv.slice(1).some(arg => arg === '--serve');

    this.settings.onLanguageChanged(newValue => this.onLanguageSettingChanged(newValue));

    this.trayIcon = this.createTrayIcon();
    this.tray = new Tray(this.trayIcon);
    this.tray.setToolTip('Just Clipboard Manager');
    this.tray.setContextMenu(this.createMenu());

    globalShortcut.register('Ctrl+Shift+Q', this.showPasteWindowAsync.bind(this));
  }


  private createTrayIcon(): NativeImage {
    let trayIconFileName = this.app.isPackaged ? 'tray.ico' : 'tray-dev.ico';
    if (process.platform == 'linux') {
      trayIconFileName = this.app.isPackaged ? 'tray.png' : 'tray-dev.png';
    }
    return nativeImage.createFromPath(path.join(this.appDir, 'assets', process.platform, trayIconFileName));
  }


  private createMenu(): Menu {
    const selectedLang = this.settings.language;
    return Menu.buildFromTemplate([
      // todo: customize tray menu, see https://github.com/max-mapper/menubar
      {
        label: this.i18n.translate('settings'),
        click: () => this.showMainWindowAsync(this.trayIcon)
      },
      {
        label: this.i18n.translate('about'),
      },
      {
        type: 'separator'
      },
      {
        label: this.i18n.translate('language'),
        submenu: [
          {
            label: 'English (United States)',
            type: 'checkbox',
            checked: selectedLang === 'en',
            click: (e) => this.setLanguageFromMenuAsync(e, 'en')
          },
          {
            label: 'українська (Україна)',
            type: 'checkbox',
            checked: selectedLang === 'uk',
            click: (e) => this.setLanguageFromMenuAsync(e, 'uk')
          }
        ]
      },
      {
        type: 'separator'
      },
      {
        label: this.i18n.translate('exit'),
        role: 'quit'
      }
    ]);
  }


  private async setLanguageFromMenuAsync(menuItem: MenuItem, lang: string) {
    const selectedLang = await this.settings.language;
    if (selectedLang === lang) {
      menuItem.checked = true;
    }
    else {
      this.settings.language = lang;
    }
  }


  private async onLanguageSettingChanged(lang: string) {
    await this.i18n.changeLanguageAsync(lang);
    this.tray.setContextMenu(this.createMenu());
  }


  private async showMainWindowAsync(icon: NativeImage) {
    const win = new BrowserWindow({
      minWidth: 800,
      minHeight: 600,
      width: 1000,
      height: 750,
      icon: icon,
      // titleBarStyle: 'hidden',
      webPreferences: {
        devTools: !this.app.isPackaged,
      },
    });
    await this.loadWindowAsync(win, 'main-window');
  }


  private async showPasteWindowAsync() {
    const win = new BrowserWindow({
      width: 400,
      height: 400,
      titleBarStyle: 'hidden',
      opacity: 1,
      webPreferences: {
        devTools: !this.app.isPackaged,
        preload: path.join(this.appDir, 'preload.js')
      }
    });
    await this.loadWindowAsync(win, 'paste-window');
  }


  private async loadWindowAsync(win: BrowserWindow, type: WindowType) {
    if (this.isServe && !this.app.isPackaged) {
      await win.loadURL(`http://localhost:4200/index.html?window=${type}`);
    }
    else {
      await win.loadFile(path.join(this.appDir, 'just-clipboard-manager/browser/index.html'), {
        query: { window: type }
      });
    }
  }
}


type WindowType = 'main-window' | 'paste-window';
