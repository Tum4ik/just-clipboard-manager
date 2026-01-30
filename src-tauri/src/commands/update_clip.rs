use crate::helpers::database::get_sqlite_db;
use config::Config;
use tauri::State;
use tauri_plugin_sql::DbInstances;

#[tauri::command]
pub async fn update_clip(
  config: State<'_, Config>,
  db_instances: State<'_, DbInstances>,
  clip_id: i64,
  plugin_id: String,
  representation_data: Vec<u8>,
  representation_metadata: String,
  representation_format_id: u32,
  representation_format_name: String,
  search_label: Option<String>,
) -> Result<(), String> {
  let db = get_sqlite_db(config, db_instances)
    .await
    .map_err(|e| format!("Can't get SQLite DB: {}", e))?;
  sqlx::query(
    "
    UPDATE clips
    SET plugin_id = ?,
        representation_data = ?,
        representation_metadata = ?,
        representation_format_id = ?,
        representation_format_name = ?,
        search_label = ?
    WHERE id = ?
    ",
  )
  .bind(plugin_id)
  .bind(representation_data)
  .bind(representation_metadata)
  .bind(representation_format_id)
  .bind(representation_format_name)
  .bind(search_label)
  .bind(clip_id)
  .execute(&db)
  .await
  .unwrap();
  Ok(())
}
