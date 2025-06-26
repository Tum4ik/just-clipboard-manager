use tauri_plugin_sql::{Migration, MigrationKind};

pub fn v1() -> Migration {
  Migration {
    version: 1,
    description: "Initial database creation",
    kind: MigrationKind::Up,
    sql: "
    CREATE TABLE clips (
      id INTEGER PRIMARY KEY,
      plugin_id TEXT,
      representation_data BLOB,
      representation_metadata TEXT,
      representation_format_id INTEGER,
      representation_format_name TEXT,
      search_label TEXT,
      clipped_at DATETIME NOT NULL DEFAULT (datetime('now','localtime'))
    );
    CREATE INDEX idx_clips_search_label ON clips (search_label);

    CREATE TABLE data_objects (
      id INTEGER PRIMARY KEY,
      format_id INTEGER NOT NULL,
      data BLOB NOT NULL,

      clip_id INTEGER NOT NULL,
      FOREIGN KEY (clip_id) REFERENCES clips(id) ON DELETE CASCADE
    );
    ",
  }
}
