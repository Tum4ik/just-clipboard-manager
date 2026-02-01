use config::Config;
use sqlx::{Pool, Sqlite};
use tauri::State;
use tauri_plugin_sql::DbInstances;

pub async fn get_sqlite_db(
  config: State<'_, Config>,
  db_instances: State<'_, DbInstances>,
) -> Result<Pool<Sqlite>, String> {
  let db_instances = db_instances.0.read().await;
  let connection_string = config
    .get_string("database.connection-string")
    .map_err(|e| format!("Failed to get DB connection string: {}", e))?;
  let tauri_plugin_sql::DbPool::Sqlite(db) = db_instances
    .get(&connection_string)
    .ok_or_else(|| format!("Database instance not found for: {}", connection_string))?;
  Ok(db.clone())
}
