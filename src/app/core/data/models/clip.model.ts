export interface Clip {
  id?: number;
  pluginId: `${string}-${string}-${string}-${string}-${string}`;
  representationData: Uint8Array;
  data: Uint8Array;
  format: string;
  searchLabel?: string;
  clippedAt: Date;
}
