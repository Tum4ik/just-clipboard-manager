import { dialog, Tray } from "electron";
import path from 'path';

export class AppTray {
  static initialize(dirname: string) {
    new AppTray(dirname);
  }

  private readonly tray: Tray;

  private constructor(private readonly dirname: string) {
    const trayIconFilePath = path.join(this.dirname, 'assets', process.platform, 'tray.ico');
    dialog.showMessageBox({
      message: trayIconFilePath
    });
    this.tray = new Tray(trayIconFilePath);
    this.tray.setToolTip('Just Clipboard Manager');
  }
}
