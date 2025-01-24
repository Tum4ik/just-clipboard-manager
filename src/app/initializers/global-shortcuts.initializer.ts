import { WebviewWindow } from '@tauri-apps/api/webviewWindow';
import { register } from '@tauri-apps/plugin-global-shortcut';

export async function initializeGlobalShortcutsAsync() {
  await register('CommandOrControl+Shift+Q', async () => {
    const pasteWindow = await WebviewWindow.getByLabel('paste-window');
    await pasteWindow?.show();
  });
}
