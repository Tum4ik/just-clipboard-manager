import { Injectable } from '@angular/core';
import { invoke } from "@tauri-apps/api/core";
import { listen } from '@tauri-apps/api/event';
import { ClipboardDataPlugin } from 'just-clipboard-manager-pdk';
import { Subject } from 'rxjs';
import { ClipsRepository } from '../../../core/data/repositories/clips.repository';
import { PluginsService } from '../../../core/services/plugins.service';

@Injectable()
export class ClipboardListener {
  constructor(
    private readonly pluginsService: PluginsService,
  ) { }

  private isListening = false;
  private readonly clipsRepository = new ClipsRepository();

  private clipboardUpdatedSubject = new Subject<void>();
  readonly clipboardUpdated$ = this.clipboardUpdatedSubject.asObservable();

  /**
   * The property is used to temporarily pause listening to the clipboard.
   * It is usually useful when the user pastes the data from the paste window and you don't want
   * to handle the data you have already in the database again.
   */
  isListeningPaused = false;

  async startListenAsync() {
    if (this.isListening) {
      return;
    }
    await listen<Map<string, number>>('clipboard-listener::available-formats', async e => {
      if (this.isListeningPaused) {
        return;
      }
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
    if (!bytesArr || bytesArr.length <= 0) {
      return;
    }
    const bytes = Uint8Array.from(bytesArr);
    const representationData = pickResult.plugin.extractRepresentationData(bytes, pickResult.format);
    const searchLabel = pickResult.plugin.getSearchLabel(bytes, pickResult.format);

    await this.clipsRepository.insertAsync({
      pluginId: pickResult.plugin.id,
      representationData: representationData.data,
      representationMetadata: representationData.metadata,
      data: bytes,
      formatId: pickResult.formatId,
      format: pickResult.format,
      searchLabel: searchLabel,
      clippedAt: new Date(),
    });
    this.clipboardUpdatedSubject.next();
  }


  private pickPlugin(availableFormats: Map<string, number>)
    : { plugin: ClipboardDataPlugin, format: string, formatId: number; } | null {

    for (const plugin of this.pluginsService.enabledPlugins) {
      for (const format of plugin.formats) {
        if (availableFormats.has(format)) {
          return { plugin, format, formatId: availableFormats.get(format)! };
        }
      }
    }

    return null;
  }
}
