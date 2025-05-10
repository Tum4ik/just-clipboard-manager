export type PluginId = `${string}-${string}-${string}-${string}-${string}`;

export type PackageJson = {
  version: string;
  pluginMetadata: {
    id: string;
    name: string;
    description?: { [lang: string]: string; };
  };
  author?: Author;
};

export type Author = { name?: string; email?: string; };
export type RepresentationData = { data: Uint8Array; metadata?: Object | undefined; };
