import { Injectable } from '@angular/core';
import { invoke } from '@tauri-apps/api/core';
import { WebviewWindow } from '@tauri-apps/api/webviewWindow';
import { PasteDataService } from './paste-data.service';
import { BehaviorSubject } from 'rxjs';

@Injectable()
export class PasteWindowVisibilityService {
  constructor(private readonly pasteDataService: PasteDataService) { }

  private pasteWindow?: WebviewWindow | null;
  
  private visibilitySubject = new BehaviorSubject<boolean>(false);
  readonly visibility$ = this.visibilitySubject.asObservable();

  async initAsync(): Promise<void> {
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

    await this.pasteWindow?.show();
    await this.pasteWindow?.setFocus();
    this.visibilitySubject.next(true);
  }

  async hideAsync() {
    await this.pasteWindow?.hide();
    this.visibilitySubject.next(false);
  }
}
