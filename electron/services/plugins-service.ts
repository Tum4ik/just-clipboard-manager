import { readdir } from 'fs/promises';
import { inject, injectable } from "inversify";
import path from 'path';
import 'reflect-metadata';
import { TYPES } from "../ioc/types";
import { pathToFileURL } from 'url';
import { ClipboardDataPlugin } from 'just-clipboard-manager-pdk';

@injectable()
export class PluginsService {
  constructor(
    @inject(TYPES.AppDir) private readonly appDir: string,
  ) { }


  private _plugins: ClipboardDataPlugin[] = [];
  
  
  get plugins(): readonly ClipboardDataPlugin[]{
    return this._plugins;
  }


  async loadPluginsAsync() {
    if (this._plugins.length > 0) {
      return;
    }

    const pluginsFolder = path.join(this.appDir, 'just-clipboard-manager', 'browser', 'plugins');
    const pluginsFiles = (await readdir(pluginsFolder, { withFileTypes: true }))
      .filter(item => item.isDirectory())
      .map(dir => path.join(pluginsFolder, dir.name, 'plugin-bundle.mjs'))
      .map(file => pathToFileURL(file, { windows: process.platform == 'win32' }).href);
    for (const pluginFile of pluginsFiles) {
      try {
        const plugin = await import(pluginFile);
        const pluginInstance: ClipboardDataPlugin = plugin.instance;
        this._plugins.push(pluginInstance);
      } catch (error) {
        // todo: track plugin load error
        console.log(error);
      }
    }
  }
}
