import { Clip } from "../models/clip.model";
import { BaseDatabaseRepository } from "./base-database.repository";

const decoder = new TextDecoder();
const encoder = new TextEncoder();

export class ClipsRepository extends BaseDatabaseRepository {
  async insertAsync(clip: Clip): Promise<void> {
    const representationData = decoder.decode(clip.representationData);
    const data = decoder.decode(clip.data);
    await this.db.execute(`
      INSERT INTO clips (plugin_id, representation_data, data, format, search_label, clipped_at)
      VALUES ($1, $2, $3, $4, $5, $6)
      `,
      [clip.pluginId, representationData, data, clip.format, clip.searchLabel, clip.clippedAt]
    );
  }

  async getClipsAsync(skip: number, take: number, search?: string): Promise<readonly Clip[]> {
    const result = await this.db.select<any[]>(`
      SELECT * FROM clips
      ${search ? "WHERE search_label LIKE '%' || $1 || '%'" : ''}
      ORDER BY clipped_at DESC
      LIMIT $2 OFFSET $3
      `,
      [search, take, skip]
    );
    return result.map(r => {
      return {
        id: r.id,
        pluginId: r.plugin_id,
        representationData: encoder.encode(r.representation_data),
        data: encoder.encode(r.data),
        format: r.format,
        searchLabel: r.search_label,
        clippedAt: r.clipped_at
      } as Clip;
    });
  }


  /* async getClipAsync(id: number): Promise<Clip | null> {
    return await this.db.queryFirstAsync<Clip>('SELECT * FROM clips WHERE id = ?', id);
  }

  async getClipsAsync(): Promise<Clip[]> {
    return await this.db.queryAsync<Clip>('SELECT * FROM clips');
  }

  async updateClipAsync(clip: Clip): Promise<void> {
    await this.db.executeAsync('UPDATE clips SET title = ?, url = ? WHERE id = ?', clip.title, clip.url, clip.id);
  }

  async deleteClipAsync(id: number): Promise<void> {
    await this.db.executeAsync('DELETE FROM clips WHERE id = ?', id);
  } */
}
