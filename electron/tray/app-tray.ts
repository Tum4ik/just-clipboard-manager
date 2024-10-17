import { Tray } from "electron";
import path from 'path';

export class AppTray {
  static initialize(dirname: string) {
    new AppTray(dirname);
  }

  private readonly tray: Tray;

  private constructor(private readonly dirname: string) {
    this.tray = new Tray(path.join(this.dirname, 'assets/tray-dev.ico'));
    this.tray.setToolTip('Just Clipboard Manager');
  }
}
