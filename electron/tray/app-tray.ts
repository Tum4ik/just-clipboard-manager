import { dialog, Tray } from "electron";
import path from 'path';

export class AppTray {
  static initialize(dirname: string) {
    new AppTray(dirname);
  }

  private readonly tray: Tray;

  private constructor(private readonly dirname: string) {
    let trayIconFileName = 'tray.ico';
    if (process.platform == 'linux' || process.platform == 'darwin') {
      trayIconFileName = 'tray.png';
    }
    const trayIconFilePath = path.join(this.dirname, 'assets', process.platform, trayIconFileName);
    dialog.showMessageBox({
      message: trayIconFilePath
    });
    this.tray = new Tray(trayIconFilePath);
    this.tray.setToolTip('Just Clipboard Manager');
  }
}
