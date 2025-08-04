import { Injectable } from '@angular/core';
import { invoke } from '@tauri-apps/api/core';
import { PhysicalPosition, PhysicalSize } from '@tauri-apps/api/dpi';
import { cursorPosition, monitorFromPoint, Window } from '@tauri-apps/api/window';
import { BehaviorSubject } from 'rxjs';
import { PasteDataService } from './paste-data.service';

@Injectable()
export class PasteWindowService {
  constructor(private readonly pasteDataService: PasteDataService) { }

  private pasteWindow?: Window | null;

  private isHideAllowed = true;

  private visibilitySubject = new BehaviorSubject<boolean>(false);
  readonly visibility$ = this.visibilitySubject.asObservable();

  async initAsync(): Promise<void> {
    if (this.pasteWindow) {
      return;
    }

    this.pasteWindow = await Window.getByLabel('paste-window');
    await this.pasteWindow?.onFocusChanged(async e => {
      if (!e.payload) {
        await this.hideAsync();
      }
    });
  }

  async showAsync() {
    if (!this.pasteWindow) {
      return;
    }

    const hwnd = await invoke<number>('get_foreground_window');
    this.pasteDataService.setPasteTargetWindowHwnd(hwnd);

    const size = await this.pasteWindow.outerSize();
    const position = await this.getWindowPosition(size);

    await this.pasteWindow.setPosition(position);
    await this.pasteWindow.show();
    await this.pasteWindow.setFocus();
    this.visibilitySubject.next(true);
  }

  async hideAsync() {
    if (this.isHideAllowed) {
      await this.pasteWindow?.hide();
      this.visibilitySubject.next(false);
    }
  }


  disallowHide() {
    this.isHideAllowed = false;
  }

  allowHide() {
    this.isHideAllowed = true;
  }

  focus() {
    this.pasteWindow?.setFocus();
  }

  async enableResizeAsync(): Promise<void> {
    await this.pasteWindow?.setResizable(true);
  }

  async disableResizeAsync(): Promise<void> {
    await this.pasteWindow?.setResizable(false);
  }


  private async getWindowPosition(windowSize: PhysicalSize): Promise<PhysicalPosition> {
    const position = await cursorPosition();
    const monitor = await monitorFromPoint(position.x, position.y);
    if (!monitor) {
      return position;
    }
    if (position.x + windowSize.width > monitor.position.x + monitor.size.width) {
      position.x = monitor.position.x + monitor.size.width - windowSize.width;
    }
    if (position.y + windowSize.height > monitor.position.y + monitor.size.height) {
      position.y = monitor.position.y + monitor.size.height - windowSize.height;
    }
    return position;
  }
}
