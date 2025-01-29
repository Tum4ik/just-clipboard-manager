import { Injectable } from '@angular/core';
import { ClipboardDataPlugin } from 'just-clipboard-manager-pdk';
import { BaseDirectory, readFile, readDir } from '@tauri-apps/plugin-fs';

@Injectable()
export class PluginsService {
  constructor(){
    this.detectPluginsAsync().then(plugins => this._plugins = plugins);
  }
  
  private _plugins?: ClipboardDataPlugin[];
  get plugins(): readonly ClipboardDataPlugin[]{
    return this._plugins ??[];
  }
  
  
  private async detectPluginsAsync(): Promise<ClipboardDataPlugin[]>{
    const pluginDirs = await readDir('plugins', {
      baseDir: BaseDirectory.Resource
    });
    const plugins: ClipboardDataPlugin[] = [];
    for (const pluginDir of pluginDirs){
      const pluginBundlePath = `plugins/${pluginDir.name}/plugin-bundle.mjs`;
      try{
        const pluginFileBytes = await readFile(pluginBundlePath, {
          baseDir: BaseDirectory.Resource
        });
        const blob = new Blob([pluginFileBytes], { type: 'application/javascript' });
        const url = URL.createObjectURL(blob);
        const pluginModule = await import(url);
        const pluginInstance: ClipboardDataPlugin = pluginModule.pluginInstance;
        plugins.push(pluginInstance);
      } catch (e){
        // todo: log error
        console.error(`Failed to load plugin from ${pluginBundlePath}`, e);
      }
    }
    return plugins;
  }
}
