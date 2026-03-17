import { BaseWindow } from "../base-window";

export abstract class PasteWindow extends BaseWindow {
  static async activate(): Promise<void> {
    await this.baseActivate('http://tauri.localhost/paste-window');
  }
}
