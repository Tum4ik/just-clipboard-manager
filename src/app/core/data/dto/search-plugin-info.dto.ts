import { PluginId } from "just-clipboard-manager-pdk";

export interface SearchPluginInfo {
  id: PluginId;
  name: string;
  description?: { [lang: string]: string; };
  version: string;
  author?: { name: string; email: string; };
  downloadLink: URL;
}
