use crate::{config::app_config::AppConfiguration, helpers::database::get_sqlite_db};
use config::Config;
use sqlx::Row;
use std::mem::discriminant;
use tauri::State;
use tauri_plugin_sql::{DbInstances, DbPool};

use serde::{Serialize, Serializer};

#[tauri::command]
pub async fn fix_migrations_checksum(
  app_config: State<'_, AppConfiguration>,
  db_instances: State<'_, DbInstances>,
) -> Result<(), String> {
  /* let db = get_sqlite_db(config, db_instances)
    .await
    .map_err(|e| format!("Can't get SQLite DB: {}", e))?;

  let query_result = sqlx::query(
    "
    SELECT checksum
    FROM _sqlx_migrations
    WHERE version = ?
    ",
  )
  .bind(1)
  .fetch_all(&db)
  .await
  .map_err(|e| e.to_string())?;

  for row in query_result {
    let checksum: &[u8] = row.get("checksum");
    println!("{checksum:?}");
  } */

  Ok(())
}
