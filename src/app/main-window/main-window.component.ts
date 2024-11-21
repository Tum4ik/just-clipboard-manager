import { Component, ElementRef, Renderer2 } from '@angular/core';
import { Buffer } from 'buffer';
import { ClipboardDataPlugin } from 'just-clipboard-manager-pdk';

@Component({
  selector: 'jcm-main-window',
  standalone: true,
  imports: [],
  templateUrl: './main-window.component.html',
  styleUrl: './main-window.component.scss'
})
export class MainWindowComponent {
  constructor(
    private readonly renderer: Renderer2,
    private readonly element: ElementRef
  ) { }

  async createComp() {
    const path = './plugins/text-plugin/bundle.mjs';
    const pluginModule = await import(path);
    const pluginInstance: ClipboardDataPlugin = pluginModule.pluginInstance;

    this.renderer.appendChild(this.element.nativeElement, pluginInstance.getRepresentationDataElement(Buffer.from("")));


  }

}
