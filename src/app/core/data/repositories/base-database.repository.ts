import { inject } from '@angular/core';
import { EnvironmentService } from '@app/core/services/environment.service';
import Database from '@tauri-apps/plugin-sql';

export abstract class BaseDatabaseRepository {
  private readonly environmentService = inject(EnvironmentService);

  private _db = new Promise<Database>(async resolve => {
    const connectionString = await this.environmentService.getDbConnectionStringAsync();
    const db = Database.get(connectionString);
    resolve(db);
  });
  protected getDbAsync(): Promise<Database> {
    return this._db;
  }

  async beginTransactionAsync(): Promise<void> {
    const db = await this._db;
    await db.execute("BEGIN TRANSACTION");
  }

  async commitAsync(): Promise<void> {
    const db = await this._db;
    await db.execute("COMMIT");
  }

  async rollbackAsync(): Promise<void> {
    const db = await this._db;
    await db.execute("ROLLBACK");
  }
}
