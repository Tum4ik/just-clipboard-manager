export interface SearchPluginInfo {
  id: `${string}-${string}-${string}-${string}-${string}`;
  name: string;
  version: string;
  downloadLink: URL;
  author?: string;
  authorEmail?: string;
  description?: { [lang: string]: string; };
}
