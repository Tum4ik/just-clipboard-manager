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
      data BLOB,
      format_id INTEGER,
      format TEXT,
      search_label TEXT,
      clipped_at TEXT
    );
    ",
  }
}
