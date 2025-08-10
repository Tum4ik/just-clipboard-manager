import { Injectable } from '@angular/core';
import { invoke } from '@tauri-apps/api/core';
import { MonitoringService } from '../../../core/services/monitoring.service';
import { ClipboardListener } from './clipboard-listener.service';

@Injectable()
export class PasteDataService {
  constructor(
    private readonly clipboardListener: ClipboardListener,
    private readonly monitoringService: MonitoringService
  ) { }

  private pasteTargetWindowHwnd?: number | null;

  setPasteTargetWindowHwnd(hwnd: number) {
    this.pasteTargetWindowHwnd = hwnd;
  }

  async pasteDataAsync(clipId: number) {
    if (!this.pasteTargetWindowHwnd) {
      return;
    }

    this.clipboardListener.isListeningPaused = true;
    try {
      await invoke('paste_clip', { clipId: clipId, targetWindowPtr: this.pasteTargetWindowHwnd });
    } catch (error) {
      this.monitoringService.error('Paste clip failed.', error);
    }
    this.clipboardListener.isListeningPaused = false;
    this.pasteTargetWindowHwnd = null;
  }
}
