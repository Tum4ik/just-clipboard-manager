import Database from '@tauri-apps/plugin-sql';

export abstract class BaseDatabaseRepository {
  constructor() {
    this.db = Database.get('sqlite:jcm-database.db');
  }

  protected readonly db: Database;

  async disposeAsync(): Promise<void> {
    await this.db.close();
  }
}
