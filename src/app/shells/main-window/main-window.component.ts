import { DOCUMENT } from '@angular/common';
import { Component, ElementRef, Inject, Renderer2 } from '@angular/core';
import { BaseDirectory, readFile, readDir } from '@tauri-apps/plugin-fs';
import { ClipboardDataPlugin } from 'just-clipboard-manager-pdk';

@Component({
  selector: 'jcm-main-window',
  imports: [],
  templateUrl: './main-window.component.html',
  styleUrl: './main-window.component.scss'
})
export class MainWindowComponent {
  constructor(
    private readonly renderer: Renderer2,
    private readonly element: ElementRef,
    @Inject(DOCUMENT) private readonly document: Document
  ) { }

  async testPlugin() {

    const pluginFileBytes = await readFile('plugins/text-plugin/plugin-bundle.mjs', {
      baseDir: BaseDirectory.Resource
    });
    const blob = new Blob([pluginFileBytes], { type: 'application/javascript' });
    const url = URL.createObjectURL(blob);
    const pluginModule = await import(url);
    const pluginInstance: ClipboardDataPlugin = pluginModule.pluginInstance;
    this.renderer.appendChild(this.element.nativeElement, pluginInstance.getRepresentationDataElement(Uint8Array.from([]), this.document));
  }
}
