import { Injectable } from '@angular/core';
import { invoke } from "@tauri-apps/api/core";
import { listen } from '@tauri-apps/api/event';
import { ClipboardDataPlugin } from 'just-clipboard-manager-pdk';
import { PluginsService } from '../../../core/services/plugins.service';

@Injectable()
export class ClipboardListener {
  constructor(private readonly pluginsService: PluginsService) { }

  private isListening = false;

  async startListenAsync() {
    if (this.isListening) {
      return;
    }
    await listen<Map<string, number>>('clipboard-listener::available-formats', async e => {
      const availableFormatsMap = new Map<string, number>(Object.entries(e.payload));
      await this.onClipboardUpdated(availableFormatsMap);
    });
    this.isListening = true;
  }


  private async onClipboardUpdated(availableFormats: Map<string, number>) {
    const pickResult = this.pickPlugin(availableFormats);
    if (!pickResult) {
      return;
    }

    const bytes = await invoke<Uint8Array>('get_clipboard_data_bytes', { format: pickResult.formatId });
    console.log(bytes);
  }


  private pickPlugin(availableFormats: Map<string, number>)
    : { plugin: ClipboardDataPlugin, formatId: number; } | null {

    for (const plugin of this.pluginsService.plugins) {
      for (const format of plugin.formats) {
        if (availableFormats.has(format)) {
          return { plugin, formatId: availableFormats.get(format)! };
        }
      }
    }

    return null;
  }
}
