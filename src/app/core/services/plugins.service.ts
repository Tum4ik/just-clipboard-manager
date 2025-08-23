import { moveItemInArray } from '@angular/cdk/drag-drop';
import { computed, Injectable, signal } from '@angular/core';
import { invoke } from '@tauri-apps/api/core';
import { emit, listen } from '@tauri-apps/api/event';
import { BaseDirectory, readDir, readFile, remove } from '@tauri-apps/plugin-fs';
import { fetch } from '@tauri-apps/plugin-http';
import { LazyStore } from '@tauri-apps/plugin-store';
import { download } from '@tauri-apps/plugin-upload';
import { ClipboardDataPlugin, PluginId } from 'just-clipboard-manager-pdk';
import { Subject } from 'rxjs';
import { GithubService } from '../../shells/main-window/services/github.service';
import { SearchPluginInfo } from '../dto/search-plugin-info.dto';
import { EnvironmentService } from './environment.service';
import { MonitoringService } from './monitoring.service';

const PLUGIN_INSTALLED_EVENT_NAME = 'plugin-installed-event';
const PLUGIN_UNINSTALLED_EVENT_NAME = 'plugin-uninstalled-event';
const PLUGIN_SETTINGS_CHANGED_EVENT_NAME = 'plugin-settings-changed-event';
const PLUGINS_ORDER_CHANGED_EVENT_NAME = 'plugins-order-changed-event';

const PLUGINS_ORDER_KEY = 'plugins-order';

const TEXT_PLUGIN_ID: PluginId = 'd930d2cd-3fd9-4012-a363-120676e22afa';

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

  // todo: maybe extract order to a separate PluginsOrderService
  private readonly pluginsOrder = signal<PluginId[]>([]);
  private readonly _plugins = new Map<PluginId, { plugin: ClipboardDataPlugin; isEnabled: boolean; }>();


  readonly installedPlugins = computed<readonly PluginWithAdditionalInfo[]>(() => {
    const orderedInstalledPlugins: PluginWithAdditionalInfo[] = [];
    for (const pluginId of this.pluginsOrder()) {
      const pluginItem = this._plugins.get(pluginId);
      if (pluginItem) {
        orderedInstalledPlugins.push({ plugin: pluginItem.plugin, get isEnabled() { return pluginItem.isEnabled; } });
      }
    }
    return orderedInstalledPlugins;
  });


  private readonly enabledPluginsTrigger = signal({});
  readonly enabledPlugins = computed<readonly ClipboardDataPlugin[]>(() => {
    this.enabledPluginsTrigger();
    const orderedEnabledPlugins: ClipboardDataPlugin[] = [];
    for (const pluginId of this.pluginsOrder()) {
      const pluginItem = this._plugins.get(pluginId);
      if (pluginItem && pluginItem.isEnabled) {
        orderedEnabledPlugins.push(pluginItem.plugin);
      }
    }
    return orderedEnabledPlugins;
  });


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

    let pluginsOrder = await this.pluginSettingsStore.get<PluginId[]>(PLUGINS_ORDER_KEY) ?? [TEXT_PLUGIN_ID];
    this.pluginsOrder.set(pluginsOrder);
    const pluginDirs = await readDir('plugins', { baseDir: BaseDirectory.Resource });
    for (const pluginDir of pluginDirs) {
      if (!pluginDir.isDirectory) {
        continue;
      }
      await this.loadPluginAsync(pluginDir.name);
    }

    let shouldSavePluginsOrder = false;
    if (this._plugins.size < pluginsOrder.length) {
      pluginsOrder = pluginsOrder.filter(id => this._plugins.has(id));
      shouldSavePluginsOrder = true;
    }
    else if (this._plugins.size > pluginsOrder.length) {
      const missingIds = [...this._plugins.keys()].filter(id => !this.pluginsOrder().includes(id));
      pluginsOrder = [...pluginsOrder, ...missingIds];
      shouldSavePluginsOrder = true;
    }

    if (shouldSavePluginsOrder) {
      this.pluginsOrder.set(pluginsOrder);
      await this.pluginSettingsStore.set(PLUGINS_ORDER_KEY, pluginsOrder);
      await this.pluginSettingsStore.save();
    }

    await listen<PluginId>(PLUGIN_INSTALLED_EVENT_NAME, async (e) => {
      await this.loadPluginAsync(e.payload);
      this.pluginInstalledSubject.next();
    });
    await listen<PluginId>(PLUGIN_UNINSTALLED_EVENT_NAME, async (e) => {
      await this.unloadPluginAsync(e.payload);
      this.pluginInstalledSubject.next();
    });
    await listen<PluginSettingsChangedPayload>(PLUGIN_SETTINGS_CHANGED_EVENT_NAME, e => {
      this.pluginSettingsChanged(e.payload);
      this.pluginSettingsChangedSubject.next();
    });
    await listen<PluginId[]>(PLUGINS_ORDER_CHANGED_EVENT_NAME, e => {
      this.pluginsOrderChanged(e.payload);
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
      await emit<PluginId>(PLUGIN_INSTALLED_EVENT_NAME, id);
      return true;
    } catch (e) {
      this.monitoringService.error(`Failed to install plugin. URL: ${url}`, e);
    }

    return false;
  }


  async uninstallPluginAsync(id: PluginId): Promise<void> {
    try {
      await remove(`plugins/${id}`, { baseDir: BaseDirectory.Resource, recursive: true });
      await emit<PluginId>(PLUGIN_UNINSTALLED_EVENT_NAME, id);
    } catch (e) {
      this.monitoringService.error(`Failed to uninstall plugin ${id}`, e);
    }
  }


  async enablePluginAsync(id: PluginId) {
    const settings = { enabled: true } as PluginSettings;
    await this.pluginSettingsStore.set(id, settings);
    await this.pluginSettingsStore.save();
    await emit<PluginSettingsChangedPayload>(PLUGIN_SETTINGS_CHANGED_EVENT_NAME, { id, settings });
  }


  async disablePluginAsync(id: PluginId) {
    const settings = { enabled: false } as PluginSettings;
    await this.pluginSettingsStore.set(id, settings);
    await this.pluginSettingsStore.save();
    await emit<PluginSettingsChangedPayload>(PLUGIN_SETTINGS_CHANGED_EVENT_NAME, { id, settings });
  }


  async changePluginsOrderAsync(fromIndex: number, toIndex: number) {
    this.pluginsOrder.update(order => {
      moveItemInArray(order, fromIndex, toIndex);
      return order;
    });
    await this.pluginSettingsStore.set(PLUGINS_ORDER_KEY, this.pluginsOrder());
    await this.pluginSettingsStore.save();
    await emit<PluginId[]>(PLUGINS_ORDER_CHANGED_EVENT_NAME, this.pluginsOrder());
  }


  private async loadPluginAsync(pluginDirName: string): Promise<void> {
    const pluginBundlePath = `plugins/${pluginDirName}/plugin-bundle.mjs`;
    try {
      const pluginFileBytes = await readFile(pluginBundlePath, {
        baseDir: BaseDirectory.Resource
      });
      const blob = new Blob([pluginFileBytes as BlobPart], { type: 'application/javascript' });
      const url = URL.createObjectURL(blob);
      const pluginModule = await import(/* @vite-ignore */url);
      const pluginInstance: ClipboardDataPlugin = pluginModule.pluginInstance;
      const pluginId = pluginInstance.id;
      let enabled = true;
      const settings = await this.pluginSettingsStore.get<PluginSettings>(pluginId);
      if (settings) {
        enabled = settings.enabled;
      }
      this._plugins.set(pluginId, { plugin: pluginInstance, isEnabled: enabled });

      if (!this.pluginsOrder().includes(pluginId)) {
        this.pluginsOrder.update(order => {
          order.unshift(pluginId);
          return [...order];
        });
        await this.pluginSettingsStore.set(PLUGINS_ORDER_KEY, this.pluginsOrder());
        await this.pluginSettingsStore.save();
      }
    } catch (e) {
      this.monitoringService.error(`Failed to load plugin from ${pluginBundlePath}`, e);
    }
  }


  private async unloadPluginAsync(id: PluginId): Promise<void> {
    try {
      // Remove plugin from memory
      this._plugins.delete(id);

      // Remove from plugins order
      const index = this.pluginsOrder().indexOf(id);
      if (index >= 0) {
        this.pluginsOrder.update(order => {
          order.splice(index, 1);
          return [...order];
        });
        await this.pluginSettingsStore.set(PLUGINS_ORDER_KEY, this.pluginsOrder());
        await this.pluginSettingsStore.save();
      }

      // Remove plugin settings
      await this.pluginSettingsStore.delete(id);
      await this.pluginSettingsStore.save();
    } catch (e) {
      this.monitoringService.error(`Failed to unload plugin ${id}`, e);
    }
  }


  private pluginSettingsChanged(payload: PluginSettingsChangedPayload) {
    const plugin = this._plugins.get(payload.id);
    if (plugin) {
      plugin.isEnabled = payload.settings.enabled;
      this.enabledPluginsTrigger.set({});
    }
  }


  private pluginsOrderChanged(pluginsOrder: PluginId[]) {
    this.pluginsOrder.set(pluginsOrder);
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
