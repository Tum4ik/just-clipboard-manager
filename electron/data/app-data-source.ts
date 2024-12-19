import { app } from 'electron';
import path from 'path';
import { DataSource } from "typeorm";
import { Clip } from "./entities/clip";

export const AppDataSource = new DataSource({
  type: 'sqlite',
  database: path.join(app.getPath('userData'), app.isPackaged ? 'database.db' : 'database-debug.db'),
  entities: [Clip],
  synchronize: true
});
