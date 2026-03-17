use tauri_plugin_sql::{Migration, MigrationKind};

pub fn v2() -> Migration {
  Migration {
    version: 2,
    description: "Add indexes for faster delete queries",
    kind: MigrationKind::Up,
    sql: "
    CREATE INDEX IF NOT EXISTS idx_clips_clipped_at ON clips (clipped_at);
    CREATE INDEX IF NOT EXISTS idx_data_objects_clip_id ON data_objects (clip_id);
    ANALYZE clips;
    ANALYZE data_objects;
    ",
  }
}
