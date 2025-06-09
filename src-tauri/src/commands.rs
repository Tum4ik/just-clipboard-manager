use clipboard_win::raw::{close, get, open, set, set_without_clear, size};
use clipboard_win::SysResult;
use config::Config;
use sqlx::Row;
use std::collections::HashMap;
use std::ffi::c_void;
use std::fs::{remove_file, File};
use std::path::{Path, PathBuf};
use tauri::{State, WebviewUrl};
use tauri_plugin_sql::DbInstances;
use tauri_plugin_sql::DbPool::Sqlite;
use windows::Win32::Foundation::HWND;
use windows::Win32::System::Threading::{AttachThreadInput, GetCurrentThreadId};
use windows::Win32::UI::Input::KeyboardAndMouse::{
  SendInput, INPUT, INPUT_KEYBOARD, KEYEVENTF_KEYUP, VK_LCONTROL, VK_V,
};
use windows::Win32::UI::WindowsAndMessaging::{
  GetForegroundWindow, GetWindowThreadProcessId, SetForegroundWindow,
};
use zip::ZipArchive;

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

#[tauri::command]
pub async fn paste_clip(
  db_instances: State<'_, DbInstances>,
  clip_id: i64,
  target_window_ptr: usize,
) -> Result<(), String> {
  let db_instances = db_instances.0.read().await;
  let Sqlite(db) = db_instances.get(DB_PATH).unwrap();
  let query_result = sqlx::query(
    "
    SELECT format_id, data
    FROM data_objects
    WHERE clip_id = ?
    ",
  )
  .bind(clip_id)
  .fetch_all(db)
  .await
  .unwrap();

  if let Err(e) = open() {
    return Err(format!("Can't open clipboard. {e}"));
  }

  let mut first_iteration = true;
  for row in query_result {
    let format_id: u32 = row.get("format_id");
    let data: &[u8] = row.get("data");
    let set_result: SysResult<()>;
    if first_iteration {
      set_result = set(format_id, data);
      first_iteration = false;
    } else {
      set_result = set_without_clear(format_id, data);
    }
    if let Err(e) = set_result {
      sentry::capture_message(
        format!("Can't set data to clipboard for format: {format_id}. {e}").as_str(),
        sentry::Level::Error,
      );
      continue;
    }
  }

  let _ = close();

  unsafe {
    let hwnd = HWND(target_window_ptr as *mut c_void);

    let hwnd_thread = GetWindowThreadProcessId(hwnd, None);
    let current_thread = GetCurrentThreadId();
    if hwnd_thread != current_thread {
      let _ = AttachThreadInput(current_thread, hwnd_thread, true);
    }

    let _ = SetForegroundWindow(hwnd);

    if hwnd_thread != current_thread {
      let _ = AttachThreadInput(current_thread, hwnd_thread, false);
    }

    let mut inputs = vec![INPUT::default(); 4];

    inputs[0].r#type = INPUT_KEYBOARD;
    inputs[0].Anonymous.ki.wVk = VK_LCONTROL;

    inputs[1].r#type = INPUT_KEYBOARD;
    inputs[1].Anonymous.ki.wVk = VK_V;

    inputs[2].r#type = INPUT_KEYBOARD;
    inputs[2].Anonymous.ki.wVk = VK_V;
    inputs[2].Anonymous.ki.dwFlags = KEYEVENTF_KEYUP;

    inputs[3].r#type = INPUT_KEYBOARD;
    inputs[3].Anonymous.ki.wVk = VK_LCONTROL;
    inputs[3].Anonymous.ki.dwFlags = KEYEVENTF_KEYUP;

    SendInput(&inputs, size_of::<INPUT>() as i32);
  }
  Ok(())
}

#[tauri::command]
pub fn get_foreground_window() -> usize {
  unsafe { GetForegroundWindow().0 as usize }
}

#[tauri::command]
pub fn open_main_window(app: tauri::AppHandle, section: Option<&str>) {
  let handle = app.clone();
  match handle
    .config()
    .app
    .windows
    .iter()
    .find(|c| c.label == "main-window")
    .cloned()
  {
    Some(mut config) => {
      if section.is_some() {
        let new_url = config.url.to_string() + "/" + section.unwrap();
        config.url = WebviewUrl::App(PathBuf::from(new_url));
      }
      std::thread::spawn(move || {
        tauri::WebviewWindowBuilder::from_config(&handle, &config)
          .unwrap()
          .build()
          .unwrap();
      });
    }
    None => {
      sentry::capture_message("Config for main window is not found", sentry::Level::Fatal);
    }
  }
}

#[tauri::command]
pub fn environment(config: State<'_, Config>) -> String {
  config.get_string("environment").unwrap()
}

#[tauri::command]
pub fn extract_and_remove_zip(zip_file_path: String, plugin_extraction_folder: String) {
  let file_path = Path::new(&zip_file_path);
  let file = File::open(file_path).unwrap();
  let mut archive = ZipArchive::new(file).unwrap();
  archive
    .extract(Path::new(&plugin_extraction_folder))
    .unwrap();
  remove_file(file_path).unwrap();
}
