import { Clip } from "./clip.model";

export interface PinnedClip {
  id: number;
  orderNextId: number | null;

  clip: Clip;
}
