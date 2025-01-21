import { listen } from '@tauri-apps/api/event';

export function initializeClipboardListener() {
  listen<string[]>('clipboard-listener::available-formats', e => {
    const availableFormats = e.payload;
    console.log(availableFormats);
  });
}
