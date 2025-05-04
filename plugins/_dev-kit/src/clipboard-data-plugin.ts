import { PluginId } from "./plugin-id";

export abstract class ClipboardDataPlugin {
  constructor(packageJson: PackageJson) {
    this._id = packageJson.pluginMetadata.id as PluginId;
    this._name = packageJson.pluginMetadata.name;
    this._description = packageJson.pluginMetadata.description;
    this._author = packageJson.author;
  }

  private _id: PluginId;
  get id(): PluginId {
    return this._id;
  }

  private _name: string;
  get name(): string {
    return this._name;
  }

  private _description: { [lang: string]: string; } | undefined;
  get description(): { [lang: string]: string; } | undefined {
    return this._description;
  }

  private _author: Author | undefined;
  get author(): Author | undefined {
    return this._author;
  }

  abstract get formats(): readonly string[];
  abstract extractRepresentationData(data: Uint8Array, format: string): RepresentationData;
  abstract getRepresentationDataElement(representationData: RepresentationData, format: string, document: Document): HTMLElement;
  abstract getSearchLabel(data: Uint8Array, format: string): string | null;
}


type PackageJson = {
  version: string;
  pluginMetadata: {
    id: string;
    name: string;
    description?: { [lang: string]: string; };
  };
  author?: Author;
};

type Author = { name?: string; email?: string; };
type RepresentationData = { data: Uint8Array; metadata?: Object; };
