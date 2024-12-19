import { readdir } from 'fs/promises';
import { inject, injectable } from "inversify";
import { ClipboardDataPlugin } from 'just-clipboard-manager-pdk';
import path from 'path';
import { pathToFileURL } from 'url';
import { TYPES } from "../ioc/types";

@injectable()
export class PluginsService {
  constructor(
    @inject(TYPES.AppDir) private readonly appDir: string,
  ) { }


  private _plugins: ClipboardDataPlugin[] = [];


  get plugins(): readonly ClipboardDataPlugin[] {
    return this._plugins;
  }


  async loadPluginsAsync() {
    if (this._plugins.length > 0) {
      return;
    }

    const pluginsFolder = path.join(this.appDir, 'plugins');
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
