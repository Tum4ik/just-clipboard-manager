import { AfterViewInit, Component, ElementRef, Renderer2, viewChild } from '@angular/core';
import { Button } from 'primeng/button';
import { Menubar } from 'primeng/menubar';

@Component({
  selector: 'jcm-main-window',
  templateUrl: './main-window.component.html',
  styleUrl: './main-window.component.scss',
  imports: [
    Menubar,
    Button
  ]
})
export class MainWindowComponent implements AfterViewInit {
  constructor(
    private readonly renderer: Renderer2
  ) {
  }

  private readonly titleBar = viewChild.required<unknown, ElementRef>('titleBar', { read: ElementRef });

  ngAfterViewInit(): void {
    const menubarContent = this.titleBar().nativeElement.querySelector('.p-menubar');
    this.renderer.setAttribute(menubarContent, 'data-tauri-drag-region', '');
  }
}
