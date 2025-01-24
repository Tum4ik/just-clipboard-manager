import { ResolveFn } from '@angular/router';
import { listen } from '@tauri-apps/api/event';

export const clipboardListenerResolver: ResolveFn<void> = async (route, state) => {
  await listen<string[]>('clipboard-listener::available-formats', e => {
    const availableFormats = e.payload;
    console.log(availableFormats);
  });
};
