import { invoke } from '@tauri-apps/api/core';
import { PluginId } from 'just-clipboard-manager-pdk';
import { Clip } from '../models/clip.model';
import { BaseDatabaseRepository } from "./base-database.repository";

export class ClipsRepository extends BaseDatabaseRepository {

  async updateAsync(clip: Clip): Promise<void> {
    await invoke('update_clip', {
      dbPath: this.db.path,
      clipId: clip.id,
      pluginId: clip.pluginId,
      representationData: clip.representationData,
      representationMetadata: JSON.stringify(clip.representationMetadata ?? {}),
      representationFormat: clip.representationFormat,
      searchLabel: clip.searchLabel,
    });
  }


  async getClipsAsync(enabledPluginIds: PluginId[], skip: number, take: number, search?: string): Promise<readonly Clip[]> {
    const result = await this.db.select<any[]>(`
      SELECT id, plugin_id, representation_data, representation_metadata, representation_format, search_label
      FROM clips
      WHERE plugin_id IN ('${enabledPluginIds.join("','")}')
      ${search ? "AND search_label LIKE '%' || $1 || '%'" : ''}
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
        representationFormat: r.representation_format,
        searchLabel: r.search_label,
      } as Clip;
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
      SET clipped_at = datetime($1, 'localtime')
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
