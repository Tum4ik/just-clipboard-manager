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
      representationFormatId: clip.representationFormatId,
      representationFormatName: clip.representationFormatName,
      searchLabel: clip.searchLabel,
    });
  }


  async getClipsAsync(enabledPluginIds: PluginId[], skip: number, take: number, search?: string): Promise<readonly Clip[]> {
    const result = await this.db.select<any[]>(
      /* sql */`
      SELECT
        id,
        plugin_id,
        representation_data,
        representation_metadata,
        representation_format_id,
        representation_format_name,
        search_label
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
        representationFormatId: r.representation_format_id,
        representationFormatName: r.representation_format_name,
        searchLabel: r.search_label,
      } as Clip;
    });
  }


  async updateClippedAtAsync(id: number, clippedAt: Date): Promise<void> {
    await this.db.execute(
      /* sql */`
      UPDATE clips
      SET clipped_at = datetime($1, 'localtime')
      WHERE id = $2
      `,
      [clippedAt, id]
    );
  }


  async getClipFullDataPreviewAsync(clipId: number): Promise<{ pluginId: PluginId; data: Uint8Array; format: string; } | null> {
    const result = await this.db.select<any[]>(
      /* sql */`
      SELECT clips.plugin_id, clips.representation_format_name, data_objects.data
      FROM clips
      JOIN data_objects
        ON
          clips.id = data_objects.clip_id
          AND clips.representation_format_id = data_objects.format_id
      WHERE clips.id = $1
      `,
      [clipId]
    );
    return {
      pluginId: result[0].plugin_id,
      data: new Uint8Array(result[0].data),
      format: result[0].representation_format_name
    };
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
