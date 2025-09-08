import Database from '@tauri-apps/plugin-sql';

export abstract class BaseDatabaseRepository {
  constructor() {
    this.db = Database.get('sqlite:jcm-database.db');
  }

  protected readonly db: Database;

  async beginTransactionAsync(): Promise<void> {
    await this.db.execute("BEGIN TRANSACTION");
  }

  async commitAsync(): Promise<void> {
    await this.db.execute("COMMIT");
  }

  async rollbackAsync(): Promise<void> {
    await this.db.execute("ROLLBACK");
  }
}
