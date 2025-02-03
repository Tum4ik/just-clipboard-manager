import { Clip } from "../models/clip.model";
import { BaseDatabaseRepository } from "./base-database.repository";

export class ClipsRepository extends BaseDatabaseRepository {

  async insertAsync(clip: Clip): Promise<void> {
    await this.db.execute(`
      INSERT INTO clips (plugin_id, representation_data, data, format, search_label, clipped_at)
      VALUES ($1, $2, $3, $4, $5, $6)
      `,
      [clip.pluginId, clip.representationData, clip.data, clip.format, clip.searchLabel, clip.clippedAt]
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
