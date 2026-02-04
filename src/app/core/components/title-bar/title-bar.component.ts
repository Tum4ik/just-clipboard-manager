import { Component, input, OnDestroy, OnInit, signal } from '@angular/core';
import { MonitoringService } from '@app/core/services/monitoring.service';
import { invoke } from '@tauri-apps/api/core';
import { UnlistenFn } from '@tauri-apps/api/event';
import { getCurrentWindow, Window } from '@tauri-apps/api/window';
import { Button } from 'primeng/button';
import { Menubar } from 'primeng/menubar';

@Component({
  selector: 'jcm-title-bar',
  templateUrl: './title-bar.component.html',
  styleUrl: './title-bar.component.scss',
  imports: [
    Menubar,
    Button,
  ]
})
export class TitleBarComponent implements OnInit, OnDestroy {
  constructor(
    private readonly monitoringService: MonitoringService,
  ) { }

  private window?: Window;
  private windowResizeListener?: UnlistenFn | undefined;

  readonly isMinimizeAvailable = input(true);

  protected readonly productName = signal<string | null>(null);
  protected readonly isWindowMaximized = signal(false);

  async ngOnInit() {
    const window = getCurrentWindow();
    if (!window) {
      this.monitoringService.fatal('Main window instance is not found');
      return;
    }

    this.window = window;
    this.windowResizeListener = await window.onResized(() => {
      window.isMaximized().then(isMaximized => this.isWindowMaximized.set(isMaximized));
    });

    invoke<string | null>('info_product_name').then(name => this.productName.set(name));
  }

  ngOnDestroy(): void {
    this.windowResizeListener?.();
  }


  protected async minimize() {
    await this.window?.minimize();
  }

  protected async maximizeOrRestore() {
    if (await this.window?.isMaximized()) {
      await this.window?.unmaximize();
    }
    else {
      await this.window?.maximize();
    }
  }

  protected async close() {
    await this.window?.close();
  }
}
