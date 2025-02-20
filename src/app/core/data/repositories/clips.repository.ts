import { ClipPreview } from "../models/clip-preview.model";
import { Clip } from "../models/clip.model";
import { BaseDatabaseRepository } from "./base-database.repository";

const decoder = new TextDecoder();
const encoder = new TextEncoder();

export class ClipsRepository extends BaseDatabaseRepository {

  async insertAsync(clip: Clip): Promise<void> {
    const representationData = decoder.decode(clip.representationData);
    const data = decoder.decode(clip.data);
    await this.db.execute(`
      INSERT INTO clips (plugin_id, representation_data, data, format_id, format, search_label, clipped_at)
      VALUES ($1, $2, $3, $4, $5, $6, $7)
      `,
      [clip.pluginId, representationData, data, clip.formatId, clip.format, clip.searchLabel, clip.clippedAt]
    );
  }


  async getClipPreviewsAsync(skip: number, take: number, search?: string): Promise<readonly ClipPreview[]> {
    const result = await this.db.select<any[]>(`
      SELECT id, plugin_id, representation_data, format_id, format, search_label
      FROM clips
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
        formatId: r.format_id,
        format: r.format,
        searchLabel: r.search_label,
      } as ClipPreview;
    });
  }


  async getClipDataAsync(id: number): Promise<Uint8Array | null> {
    const result = await this.db.select<any[]>(`
      SELECT data
      FROM clips
      WHERE id = $1
      `,
      [id]
    );
    if (result.length <= 0) {
      return null;
    }
    return encoder.encode(result[0].data);
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
