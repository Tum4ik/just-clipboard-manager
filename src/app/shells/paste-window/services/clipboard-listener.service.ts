import { DOCUMENT } from '@angular/common';
import { Inject, Injectable } from '@angular/core';
import { invoke } from "@tauri-apps/api/core";
import { listen } from '@tauri-apps/api/event';
import { ClipboardDataPlugin } from 'just-clipboard-manager-pdk';
import { ClipsRepository } from '../../../core/data/repositories/clips.repository';
import { PluginsService } from '../../../core/services/plugins.service';

@Injectable()
export class ClipboardListener {
  constructor(
    private readonly pluginsService: PluginsService,
    @Inject(DOCUMENT) private readonly document: Document
  ) { }

  private isListening = false;
  private readonly clipsRepository = new ClipsRepository();

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

    const bytesArr = await invoke<number[]>('get_clipboard_data_bytes', { format: pickResult.formatId });
    const bytes = Uint8Array.from(bytesArr);
    const representationData = pickResult.plugin.extractRepresentationData(bytes, pickResult.format);
    console.log(new TextDecoder().decode(representationData));


    await this.clipsRepository.insertAsync({
      pluginId: pickResult.plugin.id,
      representationData: representationData,
      data: bytes,
      format: pickResult.format,
      clippedAt: new Date(),
    });
  }


  private pickPlugin(availableFormats: Map<string, number>)
    : { plugin: ClipboardDataPlugin, format: string, formatId: number; } | null {

    for (const plugin of this.pluginsService.plugins) {
      for (const format of plugin.formats) {
        if (availableFormats.has(format)) {
          return { plugin, format, formatId: availableFormats.get(format)! };
        }
      }
    }

    return null;
  }
}
