import { Injectable } from '@angular/core';
import { invoke } from '@tauri-apps/api/core';
import { ClipboardListener } from './clipboard-listener.service';

@Injectable()
export class PasteDataService {
  constructor(private readonly clipboardListener: ClipboardListener) { }

  private pasteTargetWindowHwnd?: number | null;

  setPasteTargetWindowHwnd(hwnd: number) {
    this.pasteTargetWindowHwnd = hwnd;
  }

  async pasteDataAsync(data: Uint8Array, formatId: number) {
    if (!this.pasteTargetWindowHwnd) {
      return;
    }

    this.clipboardListener.isListeningPaused = true;
    await invoke('paste_data_bytes', { format: formatId, bytes: data, targetWindowPtr: this.pasteTargetWindowHwnd });
    this.clipboardListener.isListeningPaused = false;
    this.pasteTargetWindowHwnd = null;
  }
}
