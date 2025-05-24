import { Injectable } from '@angular/core';
import { invoke } from '@tauri-apps/api/core';
import { emit, listen } from '@tauri-apps/api/event';
import { BaseDirectory, readDir, readFile } from '@tauri-apps/plugin-fs';
import { fetch } from '@tauri-apps/plugin-http';
import { LazyStore } from '@tauri-apps/plugin-store';
import { download } from '@tauri-apps/plugin-upload';
import { ClipboardDataPlugin, PluginId } from 'just-clipboard-manager-pdk';
import { Subject } from 'rxjs';
import { GithubService } from '../../shells/main-window/services/github.service';
import { SearchPluginInfo } from '../data/dto/search-plugin-info.dto';
import { EnvironmentService } from './environment.service';
import { MonitoringService } from './monitoring.service';

const PLUGIN_INSTALLED_EVENT_NAME = 'plugin-loaded-event';
const PLUGIN_SETTINGS_CHANGED_EVENT_NAME = 'plugin-settings-changed-event';

@Injectable({ providedIn: 'root' })
export class PluginsService {
  constructor(
    private readonly monitoringService: MonitoringService,
    private readonly githubService: GithubService,
    private readonly environmentService: EnvironmentService
  ) {
  }

  private readonly pluginSettingsStore = new LazyStore('plugins-settings.json', { autoSave: false });
  private readonly textDecoder = new TextDecoder();

  private _plugins = new Map<PluginId, { plugin: ClipboardDataPlugin; isEnabled: boolean; }>();

  private _installedPlugins: readonly ClipboardDataPlugin[] | undefined;
  get installedPlugins(): readonly ClipboardDataPlugin[] {
    if (!this._installedPlugins) {
      this._installedPlugins = Array.from(this._plugins.values()).map(p => p.plugin);
    }
    return this._installedPlugins;
  }

  private _enabledPlugins: readonly ClipboardDataPlugin[] | undefined;
  get enabledPlugins(): readonly ClipboardDataPlugin[] {
    if (!this._enabledPlugins) {
      this._enabledPlugins = Array.from(this._plugins.values()).filter(p => p.isEnabled).map(p => p.plugin);
    }
    return this._enabledPlugins;
  }


  private readonly pluginInstalledSubject = new Subject<void>();
  readonly pluginInstalled$ = this.pluginInstalledSubject.asObservable();

  private readonly pluginSettingsChangedSubject = new Subject<void>();
  readonly pluginSettingsChanged$ = this.pluginSettingsChangedSubject.asObservable();


  private isInitialized = false;
  async initAsync(): Promise<void> {
    if (this.isInitialized) {
      return;
    }

    this.isInitialized = true;

    const pluginDirs = await readDir('plugins', {
      baseDir: BaseDirectory.Resource
    });
    for (const pluginDir of pluginDirs) {
      if (!pluginDir.isDirectory) {
        continue;
      }
      await this.loadPluginAsync(pluginDir.name);
    }
    await listen<PluginId>(PLUGIN_INSTALLED_EVENT_NAME, async (e) => {
      await this.loadPluginAsync(e.payload);
      this.pluginInstalledSubject.next();
    });
    await listen<PluginSettingsChangedPayload>(PLUGIN_SETTINGS_CHANGED_EVENT_NAME, e => {
      this.pluginSettingsChanged(e.payload);
      this.pluginSettingsChangedSubject.next();
    });
  }


  getPlugin(id: PluginId): { plugin: ClipboardDataPlugin; isEnabled: () => boolean; } | undefined {
    const pluginItem = this._plugins.get(id);
    if (!pluginItem) {
      return undefined;
    }
    return { plugin: pluginItem.plugin, isEnabled: () => pluginItem.isEnabled };
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


  async installPluginAsync(id: PluginId, url: URL): Promise<boolean> {
    let pluginsFolder = './plugins';
    if (this.environmentService.isDevelopment) {
      pluginsFolder = './target/debug/plugins';
    }

    const pluginExtractionFolder = `${pluginsFolder}/${id}`;
    const zipFilePath = `${pluginsFolder}/${id}.zip`;
    try {
      await download(url.toString(), zipFilePath);
      await invoke('extract_and_remove_zip', { zipFilePath: zipFilePath, pluginExtractionFolder: pluginExtractionFolder });
      await emit(PLUGIN_INSTALLED_EVENT_NAME, id);
      return true;
    } catch (e) {
      this.monitoringService.error(`Failed to install plugin. URL: ${url}`, e);
    }

    return false;
  }


  async enablePluginAsync(id: PluginId) {
    const settings = { enabled: true } as PluginSettings;
    await this.pluginSettingsStore.set(id, settings);
    await this.pluginSettingsStore.save();
    await emit(PLUGIN_SETTINGS_CHANGED_EVENT_NAME, { id, settings } as PluginSettingsChangedPayload);
  }


  async disablePluginAsync(id: PluginId) {
    const settings = { enabled: false } as PluginSettings;
    await this.pluginSettingsStore.set(id, settings);
    await this.pluginSettingsStore.save();
    await emit(PLUGIN_SETTINGS_CHANGED_EVENT_NAME, { id, settings } as PluginSettingsChangedPayload);
  }


  private async loadPluginAsync(pluginDirName: string): Promise<void> {
    const pluginBundlePath = `plugins/${pluginDirName}/plugin-bundle.mjs`;
    try {
      const pluginFileBytes = await readFile(pluginBundlePath, {
        baseDir: BaseDirectory.Resource
      });
      const blob = new Blob([pluginFileBytes], { type: 'application/javascript' });
      const url = URL.createObjectURL(blob);
      const pluginModule = await import(url);
      const pluginInstance: ClipboardDataPlugin = pluginModule.pluginInstance;
      const pluginId = pluginInstance.id;
      let enabled = true;
      const settings = await this.pluginSettingsStore.get<PluginSettings>(pluginId);
      if (settings) {
        enabled = settings.enabled;
      }
      this._plugins.set(pluginId, { plugin: pluginInstance, isEnabled: enabled });
      this._installedPlugins = undefined;
      this._enabledPlugins = undefined;
    } catch (e) {
      this.monitoringService.error(`Failed to load plugin from ${pluginBundlePath}`, e);
    }
  }


  private pluginSettingsChanged(payload: PluginSettingsChangedPayload) {
    const plugin = this._plugins.get(payload.id);
    if (plugin) {
      plugin.isEnabled = payload.settings.enabled;
      this._enabledPlugins = undefined;
    }
  }
}


export interface PluginSettings {
  enabled: boolean;
}

export interface PluginSettingsChangedPayload {
  id: PluginId;
  settings: PluginSettings;
}
