import { Injectable } from '@angular/core';
import { isRegistered, register } from '@tauri-apps/plugin-global-shortcut';
import { PasteWindowService } from './paste-window.service';

@Injectable()
export class GlobalShortcutsService {
  constructor(private readonly pasteWindowService: PasteWindowService) { }

  async initAsync(): Promise<void> {
    if (!await isRegistered('CommandOrControl+Shift+Q')) {
      await register('CommandOrControl+Shift+Q', async (e) => {
        if (e.state === 'Pressed') {
          await this.pasteWindowService.showAsync();
        }
      });
    }
  }
}
