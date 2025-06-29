import { AfterViewInit, Component, ElementRef, input, NgZone, OnDestroy, OnInit, Renderer2, viewChild } from '@angular/core';
import { MonitoringService } from '@core/services/monitoring.service';
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
export class TitleBarComponent implements OnInit, AfterViewInit, OnDestroy {
  constructor(
    private readonly renderer: Renderer2,
    private readonly ngZone: NgZone,
    private readonly monitoringService: MonitoringService,
  ) { }

  private readonly titleBar = viewChild.required<Button, ElementRef>('titleBar', { read: ElementRef });

  private window?: Window;
  private windowResizeListener?: UnlistenFn | undefined;

  readonly isMinimizeAvailable = input(true);

  isWindowMaximized = false;

  async ngOnInit() {
    const window = getCurrentWindow();
    if (!window) {
      this.monitoringService.fatal('Main window instance is not found');
      return;
    }

    this.window = window;
    this.windowResizeListener = await window.onResized(async () => {
      await this.ngZone.run(async () => this.isWindowMaximized = await window.isMaximized());
    });
  }

  ngAfterViewInit(): void {
    const menubarContent = this.titleBar().nativeElement.querySelector('.p-menubar');
    this.renderer.setAttribute(menubarContent, 'data-tauri-drag-region', '');
  }

  ngOnDestroy(): void {
    this.windowResizeListener?.();
  }


  async minimize() {
    await this.window?.minimize();
  }

  async maximizeOrRestore() {
    if (await this.window?.isMaximized()) {
      await this.window?.unmaximize();
    }
    else {
      await this.window?.maximize();
    }
  }

  async close() {
    await this.window?.close();
  }
}
