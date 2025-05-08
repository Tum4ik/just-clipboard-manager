import { PluginId } from "just-clipboard-manager-pdk";

export interface SearchPluginInfo {
  id: PluginId;
  name: string;
  version: string;
  downloadLink: URL;
  author?: string;
  authorEmail?: string;
  description?: { [lang: string]: string; };
}
