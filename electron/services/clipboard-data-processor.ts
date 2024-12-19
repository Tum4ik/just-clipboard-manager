import { clipboard } from "electron";
import { injectable } from "inversify";
import { Clip } from "../data/entities/clip";
import { clipRepository } from "../data/repositories/clip-repository";
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

      const clip = new Clip();
      clip.pluginId = plugin.id;
      clip.data = data;
      clip.representationData = representationData;
      // todo: search label
      clipRepository.insert(clip);

      break;
    }
  }
}
