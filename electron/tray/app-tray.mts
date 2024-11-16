import { App, BrowserWindow, globalShortcut, Menu, NativeImage, nativeImage, Tray } from "electron";
import Store from 'electron-store';
import { i18n as I18n } from 'i18next';
import path from 'path';

export class AppTray {
  private readonly store = new Store({
    name: 'settings',
    schema: {
      language: {
        type: 'string',
        default: 'en'
      }
    }
  });
  private readonly trayIcon: NativeImage;
  private readonly tray: Tray;
  private readonly isServe: boolean;

  constructor(
    private readonly app: App,
    private readonly dirname: string,
    private readonly i18n: I18n,
  ) {
    this.isServe = process.argv.slice(1).some(arg => arg === '--serve');

    this.store.onDidChange('language', (newValue, oldValue) => this.onLanguageSettingChanged(newValue as string));

    this.trayIcon = this.createTrayIcon(app);
    this.tray = new Tray(this.trayIcon);
    this.tray.setToolTip('Just Clipboard Manager');
    this.tray.setContextMenu(this.createMenu());

    globalShortcut.register('Ctrl+Shift+Q', this.showPasteWindowAsync.bind(this));
  }


  private createTrayIcon(app: App): NativeImage {
    let trayIconFileName = app.isPackaged ? 'tray.ico' : 'tray-dev.ico';
    if (process.platform == 'linux') {
      trayIconFileName = app.isPackaged ? 'tray.png' : 'tray-dev.png';
    }
    return nativeImage.createFromPath(path.join(this.dirname, 'assets', process.platform, trayIconFileName));
  }


  private createMenu(): Menu {
    const selectedLang = this.i18n.language;
    return Menu.buildFromTemplate([
      // todo: customize tray menu, see https://github.com/max-mapper/menubar
      {
        label: this.i18n.t('settings'),
        click: () => this.showMainWindowAsync(this.trayIcon)
      },
      {
        label: this.i18n.t('about')
      },
      {
        type: 'separator'
      },
      {
        label: this.i18n.t('language'),
        submenu: [
          {
            label: 'English (United States)',
            type: 'checkbox',
            checked: selectedLang === 'en',
            click: () => this.store.set('language', 'en')
          },
          {
            label: 'українська (Україна)',
            type: 'checkbox',
            checked: selectedLang === 'uk',
            click: () => this.store.set('language', 'uk')
          }
        ]
      },
      {
        type: 'separator'
      },
      {
        label: this.i18n.t('exit'),
        click: this.app.quit
      }
    ]);
  }


  private async onLanguageSettingChanged(lang: string) {
    await this.i18n.changeLanguage(lang);
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
        preload: path.join(this.dirname, 'preload.js')
      },
    });
    await this.loadWindowAsync(win, 'main-window');
  }


  private async showPasteWindowAsync() {
    const win = new BrowserWindow({
      width: 400,
      height: 400,
      titleBarStyle: 'hidden',
      opacity: .5,
      webPreferences: {
        devTools: !this.app.isPackaged
      }
    });
    await this.loadWindowAsync(win, 'paste-window');
  }


  private async loadWindowAsync(win: BrowserWindow, type: WindowType) {
    if (this.isServe && !this.app.isPackaged) {
      await win.loadURL(`http://localhost:4200/index.html?window=${type}`);
    }
    else {
      await win.loadFile(path.join(this.dirname, 'just-clipboard-manager/browser/index.html'), {
        query: { window: type }
      });
    }
  }
}


type WindowType = 'main-window' | 'paste-window';
