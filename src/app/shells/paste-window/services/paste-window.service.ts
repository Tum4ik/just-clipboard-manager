import { Injectable } from '@angular/core';
import { invoke } from '@tauri-apps/api/core';
import { WebviewWindow } from '@tauri-apps/api/webviewWindow';
import { BehaviorSubject } from 'rxjs';
import { PasteDataService } from './paste-data.service';
import { LogicalPosition } from '@tauri-apps/api/dpi';

@Injectable()
export class PasteWindowService {
  constructor(private readonly pasteDataService: PasteDataService) { }

  private pasteWindow?: WebviewWindow | null;

  private visibilitySubject = new BehaviorSubject<boolean>(false);
  readonly visibility$ = this.visibilitySubject.asObservable();

  async initAsync(): Promise<void> {
    if (this.pasteWindow) {
      return;
    }

    this.pasteWindow = await WebviewWindow.getByLabel('paste-window');
    await this.pasteWindow?.onFocusChanged(async e => {
      if (!e.payload) {
        await this.hideAsync();
      }
    });
  }

  async showAsync() {
    const hwnd = await invoke<number>('get_foreground_window');
    this.pasteDataService.setPasteTargetWindowHwnd(hwnd);

    const [x, y] = await invoke<[x: number, y: number]>('get_cursor_pos');

    // await this.pasteWindow?.setPosition(new LogicalPosition(x, y));
    await this.pasteWindow?.show();
    await this.pasteWindow?.setFocus();
    this.visibilitySubject.next(true);
  }

  async hideAsync() {
    await this.pasteWindow?.hide();
    this.visibilitySubject.next(false);
  }
}
