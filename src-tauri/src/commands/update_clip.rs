use tauri::State;
use tauri_plugin_sql::DbInstances;
use tauri_plugin_sql::DbPool::Sqlite;

#[tauri::command]
pub async fn update_clip(
  db_instances: State<'_, DbInstances>,
  db_path: &str,
  clip_id: i64,
  plugin_id: String,
  representation_data: Vec<u8>,
  representation_metadata: String,
  representation_format: String,
  search_label: Option<String>,
) -> Result<(), String> {
  let db_instances = db_instances.0.read().await;
  let Sqlite(db) = db_instances.get(db_path).unwrap();
  sqlx::query(
    "
    UPDATE clips
    SET plugin_id = ?,
        representation_data = ?,
        representation_metadata = ?,
        representation_format = ?,
        search_label = ?
    WHERE id = ?
    ",
  )
  .bind(plugin_id)
  .bind(representation_data)
  .bind(representation_metadata)
  .bind(representation_format)
  .bind(search_label)
  .bind(clip_id)
  .execute(db)
  .await
  .unwrap();
  Ok(())
}
