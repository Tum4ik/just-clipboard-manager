import { PinnedClip } from "../models/pinned-clip.model";
import { BaseDatabaseRepository } from "./base-database.repository";

export class PinnedClipsRepository extends BaseDatabaseRepository {
  async getPinnedClipsAsync(): Promise<PinnedClip[]> {
    const result = await this.db.select<any[]>(
      /* sql */`
      SELECT
        pinned_clips.id,
        pinned_clips.order_next_id,
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
        orderNextId: r.order_next_id,
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


  async updatePinnedClipOrderNextIdAsync(id: number, orderNextId: number | null): Promise<void> {
    await this.db.execute(
      /* sql */`
      UPDATE pinned_clips
      SET order_next_id = $1
      WHERE id = $2
      `,
      [orderNextId, id]
    );
  }


  async getPrevPinnedClipIdAsync(currClipId: number): Promise<number | null> {
    const result = await this.db.select<any[]>(
      /* sql */`
      SELECT id
      FROM pinned_clips
      WHERE order_next_id = $1
      `,
      [currClipId]
    );
    if (result.length < 1) {
      return null;
    }
    return result[0].id;
  }


  async getNextPinnedClipIdAsync(currClipId: number): Promise<number | null> {
    const result = await this.db.select<any[]>(
      /* sql */`
      SELECT order_next_id
      FROM pinned_clips
      WHERE id = $1
      `,
      [currClipId]
    );
    if (result.length < 1) {
      return null;
    }
    return result[0].order_next_id;
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
