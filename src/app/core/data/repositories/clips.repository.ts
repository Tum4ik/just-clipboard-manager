import { invoke } from '@tauri-apps/api/core';
import { ClipPreview } from "../models/clip-preview.model";
import { Clip } from "../models/clip.model";
import { BaseDatabaseRepository } from "./base-database.repository";

export class ClipsRepository extends BaseDatabaseRepository {

  async insertAsync(clip: Clip): Promise<void> {
    const metadataObject = clip.representationMetadata ?? {};
    await invoke('insert_bytes_data', {
      dbPath: this.db.path,
      pluginId: clip.pluginId,
      representationData: clip.representationData,
      representationMetadata: JSON.stringify(metadataObject),
      data: clip.data,
      formatId: clip.formatId,
      format: clip.format,
      searchLabel: clip.searchLabel,
      clippedAt: clip.clippedAt,
    });
  }


  async getClipPreviewsAsync(skip: number, take: number, search?: string): Promise<readonly ClipPreview[]> {
    const result = await this.db.select<any[]>(`
      SELECT id, plugin_id, representation_data, representation_metadata, format_id, format, search_label
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
        representationData: new Uint8Array(r.representation_data),
        representationMetadata: JSON.parse(r.representation_metadata),
        formatId: r.format_id,
        format: r.format,
        searchLabel: r.search_label,
      } as ClipPreview;
    });
  }


  // todo: move to rust (command)
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
    return new Uint8Array(result[0].data);
  }


  async updateClippedAtAsync(id: number, clippedAt: Date): Promise<void> {
    await this.db.execute(`
      UPDATE clips
      SET clipped_at = $1
      WHERE id = $2
      `,
      [clippedAt, id]
    );
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
