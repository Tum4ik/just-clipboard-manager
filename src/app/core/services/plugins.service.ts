import { Injectable } from '@angular/core';
import { BaseDirectory, readDir, readFile } from '@tauri-apps/plugin-fs';
import { ClipboardDataPlugin } from 'just-clipboard-manager-pdk';
import { MonitoringService } from './monitoring.service';

@Injectable({ providedIn: 'root' })
export class PluginsService {
  constructor(private readonly monitoringService: MonitoringService) {
    this.detectPluginsAsync().then(plugins => this._plugins = plugins);
  }

  private _plugins?: Map<string, ClipboardDataPlugin>;
  get plugins(): readonly ClipboardDataPlugin[] {
    if (this._plugins) {
      return Array.from(this._plugins.values());
    }
    return [];
  }

  getPlugin(id: string): ClipboardDataPlugin | undefined {
    return this._plugins?.get(id);
  }


  private async detectPluginsAsync(): Promise<Map<string, ClipboardDataPlugin>> {
    const pluginDirs = await readDir('plugins', {
      baseDir: BaseDirectory.Resource
    });
    const plugins = new Map<string, ClipboardDataPlugin>();
    for (const pluginDir of pluginDirs) {
      const pluginBundlePath = `plugins/${pluginDir.name}/plugin-bundle.mjs`;
      try {
        const pluginFileBytes = await readFile(pluginBundlePath, {
          baseDir: BaseDirectory.Resource
        });
        const blob = new Blob([pluginFileBytes], { type: 'application/javascript' });
        const url = URL.createObjectURL(blob);
        const pluginModule = await import(url);
        const pluginInstance: ClipboardDataPlugin = pluginModule.pluginInstance;
        plugins.set(pluginInstance.id, pluginInstance);
      } catch (e) {
        this.monitoringService.error(`Failed to load plugin from ${pluginBundlePath}`, e);
      }
    }
    return plugins;
  }
}
