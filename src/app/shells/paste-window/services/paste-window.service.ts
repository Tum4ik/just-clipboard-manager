import { Injectable } from '@angular/core';
import { MonitoringService } from '@app/core/services/monitoring.service';
import { PasteWindowSizingService } from '@app/core/services/paste-window-sizing.service';
import { PasteWindowSnappingService } from '@app/core/services/paste-window-snapping.service';
import { DisplayEdgePosition, SnappingMode } from '@app/core/services/settings.service';
import { invoke } from '@tauri-apps/api/core';
import { PhysicalPosition, PhysicalSize } from '@tauri-apps/api/dpi';
import { cursorPosition, monitorFromPoint, Window } from '@tauri-apps/api/window';
import { moveWindow, Position } from '@tauri-apps/plugin-positioner';
import { BehaviorSubject, firstValueFrom } from 'rxjs';
import { PasteDataService } from './paste-data.service';

@Injectable()
export class PasteWindowService {
  constructor(
    private readonly monitoringService: MonitoringService,
    private readonly pasteDataService: PasteDataService,
    private readonly pasteWindowSnappingService: PasteWindowSnappingService,
    private readonly pasteWindowSizingService: PasteWindowSizingService,
  ) { }

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

    const width = await firstValueFrom(this.pasteWindowSizingService.width$);
    const height = await firstValueFrom(this.pasteWindowSizingService.height$);
    await this.pasteWindow.setSize(new PhysicalSize(width, height));

    await this.setWindowPositionAsync(await this.pasteWindow.outerSize());

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

  async rememberWindowSizeAsync(): Promise<void> {
    const currSize = await this.pasteWindow!.innerSize();
    await this.pasteWindowSizingService.setSize(currSize.width, currSize.height);
  }


  private async setWindowPositionAsync(windowSize: PhysicalSize): Promise<void> {
    const snappingMode = await this.pasteWindowSnappingService.getSnappingModeAsync();
    switch (snappingMode) {
      default:
      case SnappingMode.MouseCursor:
        {
          const position = await this.getWindowPositionByMouseCursorAsync(windowSize);
          await this.pasteWindow!.setPosition(position);
        }
        return;
      case SnappingMode.Caret:
        {
          const position = await this.getWindowPositionByCaretAsync(windowSize);
          await this.pasteWindow!.setPosition(position);
        }
        return;
      case SnappingMode.DisplayEdges:
        await this.setWindowPositionByDisplayEdgesAsync();
        return;
    }
  }


  private async getWindowPositionByMouseCursorAsync(windowSize: PhysicalSize): Promise<PhysicalPosition> {
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

  private async getWindowPositionByCaretAsync(windowSize: PhysicalSize): Promise<PhysicalPosition> {
    try {
      const caretPos = await invoke<{ x: number; y: number; }>('get_caret_position');
      const monitor = await monitorFromPoint(caretPos.x, caretPos.y);

      if (!monitor) {
        return new PhysicalPosition(caretPos.x, caretPos.y);
      }

      let x = caretPos.x;
      let y = caretPos.y + 20; // Add some offset below the caret

      // Ensure the window stays within monitor bounds
      if (x + windowSize.width > monitor.position.x + monitor.size.width) {
        x = monitor.position.x + monitor.size.width - windowSize.width;
      }
      if (y + windowSize.height > monitor.position.y + monitor.size.height) {
        y = caretPos.y - windowSize.height - 10; // Show above caret if no space below
      }

      // Ensure window doesn't go off-screen to the left
      if (x < monitor.position.x) {
        x = monitor.position.x;
      }
      // Ensure window doesn't go above screen
      if (y < monitor.position.y) {
        y = monitor.position.y;
      }

      return new PhysicalPosition(x, y);
    } catch (err) {
      console.log(err);
      this.monitoringService.error('Failed to get caret position.', err);
      // Fallback to mouse cursor position if caret position fails
      return await this.getWindowPositionByMouseCursorAsync(windowSize);
    }
  }

  private async setWindowPositionByDisplayEdgesAsync(): Promise<void> {
    const position = await this.pasteWindowSnappingService.getDisplayEdgePositionAsync();
    switch (position) {
      default:
      case DisplayEdgePosition.TopLeft:
        await moveWindow(Position.TopLeft);
        return;
      case DisplayEdgePosition.TopRight:
        await moveWindow(Position.TopRight);
        return;
      case DisplayEdgePosition.BottomLeft:
        await moveWindow(Position.BottomLeft);
        return;
      case DisplayEdgePosition.BottomRight:
        await moveWindow(Position.BottomRight);
        return;
      case DisplayEdgePosition.TopCenter:
        await moveWindow(Position.TopCenter);
        return;
      case DisplayEdgePosition.BottomCenter:
        await moveWindow(Position.BottomCenter);
        return;
      case DisplayEdgePosition.LeftCenter:
        await moveWindow(Position.LeftCenter);
        return;
      case DisplayEdgePosition.RightCenter:
        await moveWindow(Position.RightCenter);
        return;
    }
  }
}
