import { App, BrowserWindow, Menu, Tray } from "electron";
import path from 'path';

export class AppTray {
  static initialize(app: App, dirname: string) {
    new AppTray(app, dirname);
  }

  private readonly tray: Tray;
  private readonly isServe: boolean;

  private constructor(
    private readonly app: App,
    private readonly dirname: string
  ) {
    this.isServe = process.argv.slice(1).some(arg => arg === '--serve');

    let trayIconFileName = app.isPackaged ? 'tray.ico' : 'tray-dev.ico';
    if (process.platform == 'linux') {
      trayIconFileName = app.isPackaged ? 'tray.png' : 'tray-dev.png';
    }
    const trayIconFilePath = path.join(this.dirname, 'assets', process.platform, trayIconFileName);
    this.tray = new Tray(trayIconFilePath);
    this.tray.setToolTip('Just Clipboard Manager');
    this.tray.setContextMenu(Menu.buildFromTemplate([
      // todo: customize tray menu, see https://github.com/max-mapper/menubar
      {
        label: 'Settings',
        click: this.showMainWindow.bind(this)
      },
      {
        label: 'About',
      },
      {
        type: 'separator'
      },
      {
        label: 'Language',
        submenu: [
          {
            label: 'en'
          },
          {
            label: 'uk'
          }
        ]
      },
      {
        type: 'separator'
      },
      {
        label: 'Exit',
        click: this.app.quit
      }
    ]));
  }


  private showMainWindow() {
    const win = new BrowserWindow({
      minWidth: 800,
      minHeight: 600,
      width: 1000,
      height: 750,
      // titleBarStyle: 'hidden',
      webPreferences: {
        devTools: !this.app.isPackaged
      },
    });
    this.loadWindow(win, 'main-window');
  }


  private loadWindow(win: BrowserWindow, type: WindowType) {
    if (this.isServe && !this.app.isPackaged) {
      win.loadURL(`http://localhost:4200/index.html?window=${type}`);
    }
    else {
      win.loadFile(path.join(__dirname, 'just-clipboard-manager/browser/index.html'), {
        query: { window: type }
      });
    }
  }
}


type WindowType = 'main-window' | 'paste-window';
