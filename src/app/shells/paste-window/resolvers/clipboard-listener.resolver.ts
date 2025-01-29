import { inject } from '@angular/core';
import { ResolveFn } from '@angular/router';
import { ClipboardListener } from '../services/clipboard-listener.service';

export const clipboardListenerResolver: ResolveFn<void> = async (route, state) => {
  const clipboardListener = inject(ClipboardListener);
  await clipboardListener.startListenAsync();
};
