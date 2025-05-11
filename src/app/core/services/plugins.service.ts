import { Injectable } from '@angular/core';
import { BaseDirectory, readDir, readFile } from '@tauri-apps/plugin-fs';
import { fetch } from '@tauri-apps/plugin-http';
import { ClipboardDataPlugin } from 'just-clipboard-manager-pdk';
import { GithubService } from '../../shells/main-window/services/github.service';
import { SearchPluginInfo } from '../data/dto/search-plugin-info.dto';
import { MonitoringService } from './monitoring.service';

@Injectable({ providedIn: 'root' })
export class PluginsService {
  constructor(
    private readonly monitoringService: MonitoringService,
    private readonly githubService: GithubService,
  ) {
  }

  private readonly textDecoder = new TextDecoder();

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


  private isInitialized = false;
  async initAsync(): Promise<void> {
    if (this.isInitialized) {
      return;
    }

    this.isInitialized = true;

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

    this._plugins = plugins;
  }


  async searchPluginsAsync(): Promise<SearchPluginInfo[]> {
    const base64Content = await this.githubService.getPluginsListAsBase64ContentAsync();
    if (!base64Content) {
      return [];
    }

    const jsonString = atob(base64Content);
    const jsonBytes = Uint8Array.from(jsonString, ch => ch.charCodeAt(0));
    const decodedJsonString = this.textDecoder.decode(jsonBytes);
    const jsonObjects: { info: URL; zip: URL; }[] = JSON.parse(decodedJsonString);
    const pluginsInfo: SearchPluginInfo[] = [];
    for (const { info, zip } of jsonObjects) {
      try {
        const response = await fetch(info, { method: 'GET' });
        const responseText = await response.text();
        const pluginInfo: SearchPluginInfo = JSON.parse(responseText);
        pluginInfo.downloadLink = zip;
        pluginsInfo.push(pluginInfo);
      } catch (e) {
        this.monitoringService.error(`Failed to get plugin info from ${info}`, e);
      }
    }

    return pluginsInfo;
  }


  async installPluginAsync(url: URL): Promise<boolean> {
    return true;
  }
}
