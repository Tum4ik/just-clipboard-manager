import { ipcMain } from "electron";
import { injectable } from "inversify";
import { clipRepository } from "../data/repositories/clip-repository";
import { PluginsService } from "./plugins-service";

@injectable()
export class ClipsService {
  constructor(private readonly pluginsService: PluginsService) { }
  private isInitialized = false;

  init() {
    if (this.isInitialized) {
      return;
    }
    this.isInitialized = true;

    ipcMain.handle('clips:get', (event, args) => this.getClipsAsync(args[0], args[1], args[2]));
  }

  private async getClipsAsync(skip: number, take: number, search: string): Promise<HTMLElement[]> {
    const plugins = this.pluginsService.plugins;
    const clips = await clipRepository.find({ skip: skip, take: take });

    const firstPlugin = plugins[0];
    const elements = [];
    for (const clip of clips) {
      try {
        const element = firstPlugin.getRepresentationDataElement(clip.representationData);
        elements.push(element);
      }
      catch (e) {
        console.error(e);
      }
    }

    return elements;
  }
}
