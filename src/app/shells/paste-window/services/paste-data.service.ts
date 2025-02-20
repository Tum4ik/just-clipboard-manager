import { Injectable } from '@angular/core';
import { invoke } from '@tauri-apps/api/core';

@Injectable()
export class PasteDataService {

  private pasteTargetWindowHwnd?: number | null;

  setPasteTargetWindowHwnd(hwnd: number) {
    this.pasteTargetWindowHwnd = hwnd;
  }

  async pasteDataAsync(data: Uint8Array, formatId: number) {
    if (!this.pasteTargetWindowHwnd) {
      return;
    }

    await invoke('paste_data_bytes', { format: formatId, bytes: data, targetWindowPtr: this.pasteTargetWindowHwnd });
    this.pasteTargetWindowHwnd = null;
  }
}
