import { inject } from '@angular/core';
import { EnvironmentService } from '@app/core/services/environment.service';
import Database from '@tauri-apps/plugin-sql';

export abstract class BaseDatabaseRepository {
  private readonly environmentService = inject(EnvironmentService);

  private _db?: Database;
  protected get db(): Database {
    if (!this._db) {
      this._db = Database.get(this.environmentService.dbConnectionString);
    }
    return this._db;
  }

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
