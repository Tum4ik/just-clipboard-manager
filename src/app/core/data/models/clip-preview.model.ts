import { PluginId } from "just-clipboard-manager-pdk";

export interface ClipPreview {
  id?: number;
  pluginId: PluginId;
  representationData: Uint8Array;
  representationMetadata: Object | undefined;
  formatId: number;
  format: string;
  searchLabel: string | null;
}
