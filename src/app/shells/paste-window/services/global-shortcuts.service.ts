import { Injectable } from '@angular/core';
import { isRegistered, register } from '@tauri-apps/plugin-global-shortcut';
import { PasteWindowVisibilityService } from './paste-window-visibility.service';

@Injectable()
export class GlobalShortcutsService {
  constructor(private readonly pasteWindowVisibilityService: PasteWindowVisibilityService) { }

  async initAsync(): Promise<void> {
    if (!await isRegistered('CommandOrControl+Shift+Q')) {
      await register('CommandOrControl+Shift+Q', async (e) => {
        if (e.state === 'Pressed') {
          await this.pasteWindowVisibilityService.showAsync();
        }
      });
    }
  }
}
