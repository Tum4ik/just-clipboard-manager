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

const PLUGINS_ORDER_KEY = 'plugins-order';

const TEXT_PLUGIN_ID = 'd930d2cd-3fd9-4012-a363-120676e22afa';

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

  private pluginsOrder: PluginId[] | undefined;
  private readonly _plugins = new Map<PluginId, { plugin: ClipboardDataPlugin; isEnabled: boolean; }>();

  private _installedPlugins: readonly PluginWithAdditionalInfo[] | undefined;
  get installedPlugins(): readonly PluginWithAdditionalInfo[] {
    if (!this._installedPlugins) {
      const orderedInstalledPlugins: PluginWithAdditionalInfo[] = [];
      for (const pluginId of this.pluginsOrder!) {
        const pluginItem = this._plugins.get(pluginId);
        if (pluginItem) {
          orderedInstalledPlugins.push({ plugin: pluginItem.plugin, get isEnabled() { return pluginItem.isEnabled; } });
        }
      }
      this._installedPlugins = orderedInstalledPlugins;
    }
    return this._installedPlugins;
  }

  private _enabledPlugins: readonly ClipboardDataPlugin[] | undefined;
  get enabledPlugins(): readonly ClipboardDataPlugin[] {
    if (!this._enabledPlugins) {
      const orderedEnabledPlugins: ClipboardDataPlugin[] = [];
      for (const pluginId of this.pluginsOrder!) {
        const pluginItem = this._plugins.get(pluginId);
        if (pluginItem && pluginItem.isEnabled) {
          orderedEnabledPlugins.push(pluginItem.plugin);
        }
      }
      this._enabledPlugins = orderedEnabledPlugins;
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

    this.pluginsOrder = await this.pluginSettingsStore.get<PluginId[]>(PLUGINS_ORDER_KEY);
    let shouldSavePluginsOrder = false;
    if (!this.pluginsOrder) {
      this.pluginsOrder = [TEXT_PLUGIN_ID];
      shouldSavePluginsOrder = true;
    }

    const pluginDirs = await readDir('plugins', { baseDir: BaseDirectory.Resource });
    for (const pluginDir of pluginDirs) {
      if (!pluginDir.isDirectory) {
        continue;
      }
      const pluginId = await this.loadPluginAsync(pluginDir.name);
      if (pluginId && !this.pluginsOrder.includes(pluginId)) {
        this.pluginsOrder.unshift(pluginId);
        shouldSavePluginsOrder = true;
      }
    }

    if (shouldSavePluginsOrder) {
      await this.pluginSettingsStore.set(PLUGINS_ORDER_KEY, this.pluginsOrder);
      await this.pluginSettingsStore.save();
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


  private async loadPluginAsync(pluginDirName: string): Promise<PluginId | undefined> {
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
      return pluginId;
    } catch (e) {
      this.monitoringService.error(`Failed to load plugin from ${pluginBundlePath}`, e);
    }

    return undefined;
  }


  private pluginSettingsChanged(payload: PluginSettingsChangedPayload) {
    const plugin = this._plugins.get(payload.id);
    if (plugin) {
      plugin.isEnabled = payload.settings.enabled;
      this._enabledPlugins = undefined;
    }
  }
}


export interface PluginWithAdditionalInfo {
  plugin: ClipboardDataPlugin;
  get isEnabled(): boolean;
}

export interface PluginSettings {
  enabled: boolean;
}

export interface PluginSettingsChangedPayload {
  id: PluginId;
  settings: PluginSettings;
}
