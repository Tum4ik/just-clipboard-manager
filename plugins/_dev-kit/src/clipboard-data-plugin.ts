import { Author, PackageJson, PluginId, RepresentationData } from "./types";

export abstract class ClipboardDataPlugin {
  constructor(pluginPackageJson: PackageJson) {
    this._id = pluginPackageJson.pluginMetadata.id as PluginId;
    this._name = pluginPackageJson.pluginMetadata.name;
    this._version = pluginPackageJson.version;
    this._description = pluginPackageJson.pluginMetadata.description;
    this._author = pluginPackageJson.author;
  }

  private _id: PluginId;
  get id(): PluginId {
    return this._id;
  }

  private _name: string;
  get name(): string {
    return this._name;
  }

  private _version: string;
  get version(): string {
    return this._version;
  }

  private _description: { [lang: string]: string; } | undefined;
  get description(): { [lang: string]: string; } | undefined {
    return this._description;
  }

  private _author: Author | undefined;
  get author(): Author | undefined {
    return this._author;
  }

  abstract get representationFormats(): readonly string[];
  abstract get formatsToSave(): readonly string[];
  abstract extractRepresentationData(data: Uint8Array, format: string): { representationData: RepresentationData, searchLabel?: string; };
  abstract getRepresentationDataElement(representationData: RepresentationData, format: string, document: Document): HTMLElement;
  abstract getFullDataPreviewElement(data: Uint8Array, format: string, document: Document): HTMLElement;
}
