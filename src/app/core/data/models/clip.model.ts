import { ClipPreview } from "./clip-preview.model";

export interface Clip extends ClipPreview {
  data: Uint8Array;
  clippedAt: Date;
}
