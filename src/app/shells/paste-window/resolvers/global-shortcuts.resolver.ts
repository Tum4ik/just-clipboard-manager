import { ResolveFn } from '@angular/router';
import { WebviewWindow } from '@tauri-apps/api/webviewWindow';
import { isRegistered, register } from '@tauri-apps/plugin-global-shortcut';

export const globalShortcutsResolver: ResolveFn<void> = async (route, state) => {
  const pasteWindow = await WebviewWindow.getByLabel('paste-window');
  await pasteWindow?.onFocusChanged(async e => {
    if (!e.payload) {
      await pasteWindow.hide();
    }
  });
  if (!await isRegistered('CommandOrControl+Shift+Q')) {
    await register('CommandOrControl+Shift+Q', async () => {
      await pasteWindow?.show();
      await pasteWindow?.setFocus();
    });
  }
};
