import { Injectable } from "@angular/core";
import { PinnedClip } from "../models/pinned-clip.model";
import { BaseDatabaseRepository } from "./base-database.repository";

@Injectable({ providedIn: 'root' })
export class PinnedClipsRepository extends BaseDatabaseRepository {
  async getPinnedClipsAsync(): Promise<PinnedClip[]> {
    const result = await this.db.select<any[]>(
      /* sql */`
      SELECT
        pinned_clips.id,
        clips.plugin_id,
        clips.representation_data,
        clips.representation_metadata,
        clips.representation_format_id,
        clips.representation_format_name,
        clips.search_label
      FROM pinned_clips
      JOIN clips ON pinned_clips.id = clips.id
      `
    );
    return result.map(r => {
      return {
        id: r.id,
        clip: {
          id: r.id,
          pluginId: r.plugin_id,
          representationData: new Uint8Array(r.representation_data),
          representationMetadata: JSON.parse(r.representation_metadata),
          representationFormatId: r.representation_format_id,
          representationFormatName: r.representation_format_name,
          searchLabel: r.search_label,
        }
      } as PinnedClip;
    });
  }


  async addPinnedClipAsync(id: number): Promise<void> {
    await this.db.execute(
      /* sql */`
      INSERT INTO pinned_clips (id)
      VALUES ($1)
      `,
      [id]
    );
  }


  async deletePinnedClipAsync(id: number): Promise<void> {
    await this.db.execute(
      /* sql */`
      DELETE FROM pinned_clips
      WHERE id = $1
      `,
      [id]
    );
  }
}
