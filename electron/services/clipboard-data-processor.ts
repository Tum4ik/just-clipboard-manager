import { clipboard } from "electron";
import { injectable } from "inversify";
import 'reflect-metadata';
import { PluginsService } from "./plugins-service";

@injectable()
export class ClipboardDataProcessor {
  constructor(
    private readonly pluginsService: PluginsService,
  ) { }


  processCurrentItem() {
    const itemFormats = new Set(clipboard.availableFormats());
    for (const plugin of this.pluginsService.plugins) {
      const pluginFormats = plugin.formats;
      const intersection = pluginFormats.filter(pf => itemFormats.has(pf));
      if (intersection.length == 0) {
        continue;
      }

      const data = plugin.extractData(clipboard);
      const representationData = plugin.extractRepresentationData(clipboard);

      break;
    }
  }
}
