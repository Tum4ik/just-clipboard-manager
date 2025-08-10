import { Injectable } from '@angular/core';
import { invoke } from "@tauri-apps/api/core";
import { listen } from '@tauri-apps/api/event';
import { ClipboardDataPlugin } from 'just-clipboard-manager-pdk';
import { Subject } from 'rxjs';
import { ClipsRepository } from '../../../core/data/repositories/clips.repository';
import { MonitoringService } from '../../../core/services/monitoring.service';
import { PluginsService } from '../../../core/services/plugins.service';

@Injectable()
export class ClipboardListener {
  constructor(
    private readonly monitoringService: MonitoringService,
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

    try {
      const [clipId, representationBytes] = await invoke<[clipId: number, representationBytes: number[]]>(
        'save_data_objects_and_get_representation_bytes',
        {
          representationFormatId: pickResult.formatId,
          formatsToSave: pickResult.plugin.formatsToSave.map(f => availableFormats.get(f))
        }
      );

      const bytes = Uint8Array.from(representationBytes);
      const { representationData, searchLabel } = pickResult.plugin.extractRepresentationData(bytes, pickResult.formatName);

      await this.clipsRepository.updateAsync({
        id: clipId,
        pluginId: pickResult.plugin.id,
        representationData: representationData.data,
        representationMetadata: representationData.metadata,
        representationFormatId: pickResult.formatId,
        representationFormatName: pickResult.formatName,
        searchLabel: searchLabel,
      });
      this.clipboardUpdatedSubject.next();
    } catch (error) {
      this.monitoringService.error('Clipboard item handling failed.', error);
    }
  }


  private pickPlugin(availableFormats: Map<string, number>)
    : { plugin: ClipboardDataPlugin, formatName: string, formatId: number; } | null {

    for (const plugin of this.pluginsService.enabledPlugins) {
      for (const format of plugin.representationFormats) {
        if (availableFormats.has(format)) {
          return { plugin, formatName: format, formatId: availableFormats.get(format)! };
        }
      }
    }

    return null;
  }
}
