export interface ClipPreview {
  id?: number;
  pluginId: `${string}-${string}-${string}-${string}-${string}`;
  representationData: Uint8Array;
  formatId: number;
  format: string;
  searchLabel?: string;
}
