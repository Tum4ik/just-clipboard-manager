import { PluginId } from "just-clipboard-manager-pdk";

export interface Clip {
  id?: number;
  pluginId: PluginId;
  representationData: Uint8Array;
  representationMetadata: Object | undefined;
  representationFormatId: number;
  representationFormatName: string;
  searchLabel: string | undefined;
}
