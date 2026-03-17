import { DataTable, Given } from "@wdio/cucumber-framework";
import tauriConfJson from "../../src-tauri/tauri.e2e.conf.json";
import { MainWindow } from "../page-objects/main-window/main-window";
import { PasteWindow } from "../page-objects/paste-window/paste-window";

Given('the Main window is activated',
  async () => await MainWindow.activate()
);

Given('the Paste window is activated',
  async () => await PasteWindow.activate()
);

Given('the following text clipboard items:', async (table: DataTable) => {
  const Database = await import('better-sqlite3').then(sql => sql.default);

  const dbFilePath = await getDbFilePathAsync();
  if (!dbFilePath) {
    throw new Error("Can't get DB file path.");
  }
  const db = new Database(dbFilePath, { fileMustExist: true });

  const insert = db.prepare('INSERT INTO clips (search_label) VALUES (@searchLabel)');
  const insertMany = db.transaction((clips) => {
    for (const clip of clips) insert.run(clip);
  });

  const clips = table.hashes().map(h => ({ searchLabel: h['search label'] }));
  insertMany(clips);
});


async function getDbFilePathAsync(): Promise<string | undefined> {
  const path = await import('path');
  const os = await import('os');

  let dbFilePath: string | undefined;
  switch (os.platform()) {
    case 'win32':
      const appDataDir = process.env['APPDATA'] ?? path.join(os.homedir(), 'AppData', 'Roaming');
      dbFilePath = path.join(appDataDir, tauriConfJson.identifier, 'jcm-database.db');
      break;
  }

  return dbFilePath;
}
