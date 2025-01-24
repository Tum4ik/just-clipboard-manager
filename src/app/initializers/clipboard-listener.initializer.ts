import { listen } from '@tauri-apps/api/event';

export async function initializeClipboardListenerAsync() {
  await listen<string[]>('clipboard-listener::available-formats', e => {
    const availableFormats = e.payload;
    console.log(availableFormats);
  });
}
