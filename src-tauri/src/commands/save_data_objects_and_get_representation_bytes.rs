use clipboard_win::raw::{close, get, open, size};
use std::collections::HashMap;
use tauri::State;
use tauri_plugin_sql::DbInstances;
use tauri_plugin_sql::DbPool::Sqlite;

use crate::constants::DB_PATH;

#[tauri::command]
pub async fn save_data_objects_and_get_representation_bytes(
  db_instances: State<'_, DbInstances>,
  representation_format: u32,
  formats_to_save: Vec<u32>,
) -> Result<(i64, Vec<u8>), String> {
  let mut representation_bytes: Vec<u8> = vec![];
  let mut bytes_map = HashMap::<u32, Vec<u8>>::new();

  if let Err(e) = open() {
    return Err(format!("Can't open clipboard. {e}"));
  }

  for format in formats_to_save {
    let size_result = size(format);
    if let None = size_result {
      sentry::capture_message(
        format!("Can't get size for clipboard format: {format}.").as_str(),
        sentry::Level::Error,
      );
      continue;
    }

    let expected_size = size_result.unwrap().get();
    let mut bytes = vec![0u8; expected_size];

    let get_result = get(format, &mut bytes);
    if let Err(e) = get_result {
      sentry::capture_message(
        format!("Can't get data from clipboard for format: {format}. {e}").as_str(),
        sentry::Level::Error,
      );
      continue;
    }

    let read_size = get_result.unwrap();
    if expected_size <= 0 || expected_size != read_size {
      sentry::capture_message(
        format!("Incorrect format size. Format: {format}. Expected size: {expected_size}. Read size: {read_size}").as_str(),
        sentry::Level::Error,
      );
      continue;
    }

    if format == representation_format {
      representation_bytes = bytes.clone();
    }

    bytes_map.insert(format, bytes.clone());
  }

  let _ = close();

  if representation_bytes.len() <= 0 {
    return Err(format!(
      "Can't get bytes for representation format: {representation_format}"
    ));
  }

  let db_instances = db_instances.0.read().await;
  let Sqlite(db) = db_instances.get(DB_PATH).unwrap();
  let mut transaction = db.begin().await.unwrap();

  let query_result = sqlx::query(
    "
    INSERT INTO clips DEFAULT VALUES
    ",
  )
  .execute(transaction.as_mut())
  .await
  .unwrap();

  let clip_id = query_result.last_insert_rowid();

  for (format, bytes) in bytes_map {
    sqlx::query(
      "
      INSERT INTO data_objects (
        format_id,
        data,
        clip_id
      )
      VALUES (?, ?, ?)
      ",
    )
    .bind(format)
    .bind(bytes)
    .bind(clip_id)
    .execute(transaction.as_mut())
    .await
    .unwrap();
  }

  transaction.commit().await.unwrap();

  Ok((clip_id, representation_bytes))
}
