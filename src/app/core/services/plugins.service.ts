import { moveItemInArray } from '@angular/cdk/drag-drop';
import { computed, inject, Injectable, Service, signal } from '@angular/core';
import { invoke } from '@tauri-apps/api/core';
import { emit, Event, listen } from '@tauri-apps/api/event';
import { BaseDirectory, readDir, readFile, remove } from '@tauri-apps/plugin-fs';
import { fetch } from '@tauri-apps/plugin-http';
import { LazyStore } from '@tauri-apps/plugin-store';
import { download } from '@tauri-apps/plugin-upload';
import { ClipboardDataPlugin, PluginId } from 'just-clipboard-manager-pdk';
import { BehaviorSubject, map, merge, Observable, startWith, Subject } from 'rxjs';
import { SearchPluginInfo } from '../dto/search-plugin-info.dto';
import { GlobalStateService } from './base/global-state-service';
import { EnvironmentService } from './environment.service';
import { GithubService } from './github.service';
import { MonitoringService } from './monitoring.service';

const PLUGINS_ORDER_KEY = 'plugins-order';

const TEXT_PLUGIN_ID: PluginId = 'd930d2cd-3fd9-4012-a363-120676e22afa';

const PLUGINS_DIR_NAME = 'plugins';

@Service()
export class PluginsService {
  private readonly monitoringService = inject(MonitoringService);
  private readonly environmentService = inject(EnvironmentService);
  private readonly globalStateService = inject(GlobalStateService);
  private readonly githubService = inject(GithubService);

  private readonly pluginSettingsStore = new LazyStore('plugins-settings.json', {
    autoSave: false,
    defaults: {
      [PLUGINS_ORDER_KEY]: [TEXT_PLUGIN_ID]
    }
  });

  private readonly pluginsMap = new Map<PluginId, { plugin: ClipboardDataPlugin; isEnabled: BehaviorSubject<boolean>; }>();
  private readonly pluginsOrder = new BehaviorSubject<PluginId[] | null>(null);

  private readonly pluginInstalledGlobalEvent = this.globalStateService.registerGlobalObservable(
    'plugin-installed-global-event', this.pluginInstalledAsync.bind(this)
  );
  private readonly pluginUnloadGlobalTrigger = this.globalStateService.registerGlobalObservable(
    'plugin-unload-global-trigger', this.unloadPluginAsync.bind(this)
  );
  private readonly pluginSettingsChangedGlobalEvent = this.globalStateService.registerGlobalObservable(
    'plugin-settings-changed-global-event', this.pluginSettingsChangedAsync.bind(this)
  );
  private readonly pluginsOrderChangedGlobalEvent = this.globalStateService.registerGlobalObservable(
    'plugins-order-changed-global-event', this.pluginsOrderChangedAsync.bind(this)
  );


  private readonly pluginsListChangedSubject = new Subject<void>();
  readonly pluginsListChanged = this.pluginsListChangedSubject.asObservable();

  private readonly pluginSettingsChangedSubject = new Subject<void>();
  readonly pluginSettingsChanged = this.pluginSettingsChangedSubject.asObservable();


  readonly installedPlugins = merge(
    this.pluginsListChanged
  ).pipe(
    startWith(undefined),
    map(() => {
      const installedPlugins: PluginInfo[] = [];
      for (const pluginItem of this.pluginsMap.values()) {
        installedPlugins.push({
          plugin: pluginItem.plugin,
          isEnabled: pluginItem.isEnabled.asObservable()
        });
      }
      return installedPlugins;
    })
  );

  readonly installedOrderedPlugins = merge(
    this.pluginsOrder
  ).pipe(
    startWith(undefined),
    map((pluginsOrder) => {
      pluginsOrder ??= [];
      const orderedPlugins: PluginInfo[] = [];
      for (const pluginId of pluginsOrder) {
        const pluginItem = this.pluginsMap.get(pluginId);
        if (pluginItem) {
          orderedPlugins.push({
            plugin: pluginItem.plugin,
            isEnabled: pluginItem.isEnabled.asObservable()
          });
        }
      }
      return orderedPlugins;
    })
  );


  private isInitialized = false;
  async initAsync(): Promise<void> {
    if (this.isInitialized) {
      return;
    }

    this.isInitialized = true;

    const pluginDirs = (await readDir(PLUGINS_DIR_NAME, { baseDir: BaseDirectory.Resource })).filter(d => d.isDirectory);
    for (const pluginDir of pluginDirs) {
      await this.loadPluginAsync(pluginDir.name, false);
    }

    let pluginsOrder = await this.pluginSettingsStore.get<PluginId[]>(PLUGINS_ORDER_KEY) ?? [];
    let shouldSavePluginsOrder = false;
    if (this.pluginsMap.size < pluginsOrder.length) {
      pluginsOrder = pluginsOrder.filter(id => this.pluginsMap.has(id));
      shouldSavePluginsOrder = true;
    }
    else if (this.pluginsMap.size > pluginsOrder.length) {
      const missingIds = this.pluginsMap.keys().filter(id => !pluginsOrder.includes(id));
      pluginsOrder = [...missingIds, ...pluginsOrder];
      shouldSavePluginsOrder = true;
    }

    if (shouldSavePluginsOrder) {
      await this.pluginSettingsStore.set(PLUGINS_ORDER_KEY, pluginsOrder);
      await this.pluginSettingsStore.save();
    }

    this.pluginsListChangedSubject.next();
    this.pluginsOrder.next(pluginsOrder);
  }


  getPlugin(id: PluginId): PluginInfo | null {
    const pluginItem = this.pluginsMap.get(id);
    if (!pluginItem) {
      return null;
    }
    return { plugin: pluginItem.plugin, isEnabled: pluginItem.isEnabled.asObservable() };
  }


  async searchPluginsAsync(): Promise<SearchPluginInfo[]> {
    const base64Content = await this.githubService.getPluginsListAsBase64ContentAsync();
    // this.githubService.getPluginsLatestReleasesTagNamesAsync();
    return [];
  }


  async installPluginAsync(id: PluginId, url: URL): Promise<boolean> {
    let pluginsFolder = `./${PLUGINS_DIR_NAME}`;
    if (await this.environmentService.isDevelopmentAsync()) {
      pluginsFolder = `./target/debug/${PLUGINS_DIR_NAME}`;
    }

    const pluginExtractionFolder = `${pluginsFolder}/${id}`;
    const zipFilePath = `${pluginsFolder}/${id}.zip`;
    try {
      await download(url.toString(), zipFilePath);
      await invoke('extract_and_remove_zip', { zipFilePath: zipFilePath, pluginExtractionFolder: pluginExtractionFolder });
      await this.pluginInstalledGlobalEvent.invokeAsync(id);
      return true;
    } catch (error) {
      this.monitoringService.error(`Failed to install plugin. URL: ${url}`, error);
      return false;
    }
  }


  async uninstallPluginAsync(id: PluginId): Promise<void> {
    try {
      await remove(`${PLUGINS_DIR_NAME}/${id}`, { baseDir: BaseDirectory.Resource, recursive: true });
      const pluginsOrder = this.pluginsOrder.getValue();
      if (pluginsOrder) {
        const pluginIndex = pluginsOrder.indexOf(id);
        if (pluginIndex >= 0) {
          pluginsOrder.splice(pluginIndex, 1);
          await this.pluginSettingsStore.set(PLUGINS_ORDER_KEY, pluginsOrder);
          await this.pluginSettingsStore.save();

          await this.pluginSettingsStore.delete(id);
          await this.pluginSettingsStore.save();
        }
      }
    } catch (error) {
      this.monitoringService.error(`Failed to remove plugin files ${id}`, error);
      return;
    }
    await this.pluginUnloadGlobalTrigger.invokeAsync(id);
  }


  async enablePluginAsync(id: PluginId): Promise<void> {
    await this.changePluginSettingsAsync(id, { enabled: true });
  }

  async disablePluginAsync(id: PluginId): Promise<void> {
    await this.changePluginSettingsAsync(id, { enabled: false });
  }

  private async changePluginSettingsAsync(id: PluginId, settings: PluginSettings): Promise<void> {
    await this.pluginSettingsStore.set(id, settings);
    await this.pluginSettingsStore.save();
    await this.pluginSettingsChangedGlobalEvent.invokeAsync({ id: id, settings: settings });
  }


  async changePluginsOrderAsync(fromIndex: number, toIndex: number): Promise<void> {
    const order = this.pluginsOrder.getValue();
    if (!order) {
      return;
    }

    moveItemInArray(order, fromIndex, toIndex);
    await this.pluginSettingsStore.set(PLUGINS_ORDER_KEY, order);
    await this.pluginSettingsStore.save();
    await this.pluginsOrderChangedGlobalEvent.invokeAsync(order);
  }


  isBuiltInPlugin(pluginId: PluginId): boolean {
    return pluginId === TEXT_PLUGIN_ID;
  }


  private async loadPluginAsync(pluginDirName: string, notifyPluginsOrder: boolean): Promise<void> {
    const pluginBundlePath = `${PLUGINS_DIR_NAME}/${pluginDirName}/plugin-bundle.mjs`;
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
      this.pluginsMap.set(pluginId, { plugin: pluginInstance, isEnabled: new BehaviorSubject(enabled) });

      let pluginsOrder = this.pluginsOrder.getValue();
      if (pluginsOrder && !pluginsOrder.includes(pluginId)) {
        pluginsOrder = [pluginId, ...pluginsOrder];
        await this.pluginSettingsStore.set(PLUGINS_ORDER_KEY, pluginsOrder);
        await this.pluginSettingsStore.save();
        if (notifyPluginsOrder) {
          this.pluginsOrder.next(pluginsOrder);
        }
      }
    } catch (error) {
      this.monitoringService.error(`Failed to load plugin from ${pluginBundlePath}`, error);
    }
  }


  private async pluginInstalledAsync(e: Event<PluginId>) {
    await this.loadPluginAsync(e.payload, true);
    this.pluginsListChangedSubject.next();
  }


  private async unloadPluginAsync(e: Event<PluginId>) {
    const id = e.payload;

    // delete from internal plugins cache
    this.pluginsMap.delete(id);
    this.pluginsListChangedSubject.next();

    // update plugins order
    const pluginsOrder = await this.pluginSettingsStore.get<PluginId[]>(PLUGINS_ORDER_KEY) ?? [];
    this.pluginsOrder.next(pluginsOrder);
  }


  private async pluginSettingsChangedAsync(e: Event<PluginSettingsChangedPayload>) {
    const plugin = this.pluginsMap.get(e.payload.id);
    if (plugin) {
      plugin.isEnabled.next(e.payload.settings.enabled);
      this.pluginSettingsChangedSubject.next();
    }
  }


  private async pluginsOrderChangedAsync(e: Event<PluginId[]>) {
    this.pluginsOrder.next(e.payload);
  }
}






const PLUGIN_INSTALLED_EVENT_NAME = 'plugin-installed-event';
const PLUGIN_UNINSTALLED_EVENT_NAME = 'plugin-uninstalled-event';
const PLUGIN_SETTINGS_CHANGED_EVENT_NAME = 'plugin-settings-changed-event';
const PLUGINS_ORDER_CHANGED_EVENT_NAME = 'plugins-order-changed-event';



@Injectable({ providedIn: 'root' })
export class _PluginsService {
  constructor(
    private readonly monitoringService: MonitoringService,
    private readonly githubService: GithubService,
    private readonly environmentService: EnvironmentService
  ) {
  }

  private readonly pluginSettingsStore = new LazyStore('plugins-settings.json', { defaults: {}, autoSave: false });
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

  private readonly pluginUninstalledSubject = new Subject<void>();
  readonly pluginUninstalled$ = this.pluginUninstalledSubject.asObservable();

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
      this.pluginUninstalledSubject.next();
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
    if (await this.environmentService.isDevelopmentAsync()) {
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


  isBuiltInPlugin(pluginId: PluginId): boolean {
    return pluginId === TEXT_PLUGIN_ID;
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


export interface PluginInfo {
  plugin: ClipboardDataPlugin;
  isEnabled: Observable<boolean>;
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
