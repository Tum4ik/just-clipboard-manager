import { inject } from '@angular/core';
import { ResolveFn } from '@angular/router';
import { invoke } from '@tauri-apps/api/core';
import { WebviewWindow } from '@tauri-apps/api/webviewWindow';
import { isRegistered, register } from '@tauri-apps/plugin-global-shortcut';
import { PasteDataService } from '../services/paste-data.service';

export const globalShortcutsResolver: ResolveFn<void> = async (route, state) => {
  const pasteDataService = inject(PasteDataService);

  const pasteWindow = await WebviewWindow.getByLabel('paste-window');
  await pasteWindow?.onFocusChanged(async e => {
    if (!e.payload) {
      await pasteWindow.hide();
    }
  });
  if (!await isRegistered('CommandOrControl+Shift+Q')) {
    await register('CommandOrControl+Shift+Q', async (e) => {
      if (e.state === 'Pressed') {
        const hwnd = await invoke<number>('get_foreground_window');
        pasteDataService.setPasteTargetWindowHwnd(hwnd);

        await pasteWindow?.show();
        await pasteWindow?.setFocus();
      }
    });
  }
};
