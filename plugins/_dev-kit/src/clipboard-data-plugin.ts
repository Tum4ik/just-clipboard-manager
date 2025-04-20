export abstract class ClipboardDataPlugin {
  constructor(packageJson: PackageJson) {
    this._id = packageJson.pluginMetadata.id;
    this._name = packageJson.pluginMetadata.name;
    this._description = packageJson.pluginMetadata.description;
    this._author = packageJson.author;
  }

  private _id: `${string}-${string}-${string}-${string}-${string}`;
  get id(): `${string}-${string}-${string}-${string}-${string}` {
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
  abstract extractRepresentationData(data: Uint8Array, format: string): Uint8Array;
  abstract getRepresentationDataElement(representationData: Uint8Array, format: string, document: Document): HTMLElement;
  abstract getSearchLabel(data: Uint8Array, format: string): string;
}


type PackageJson = {
  version: string;
  pluginMetadata: {
    id: `${string}-${string}-${string}-${string}-${string}`;
    name: string;
    description?: { [lang: string]: string; };
  };
  author?: Author;
};

type Author = { name?: string; email?: string; };
