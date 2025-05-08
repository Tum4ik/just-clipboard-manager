use clipboard_win::raw::{close, get, open, set, size};
use std::ffi::c_void;
use std::path::PathBuf;
use tauri::{State, WebviewUrl};
use tauri_plugin_sql::DbInstances;
use tauri_plugin_sql::DbPool::Sqlite;
use windows::Win32::Foundation::HWND;
use windows::Win32::System::Threading::{AttachThreadInput, GetCurrentThreadId};
use windows::Win32::UI::Input::KeyboardAndMouse::{
  SendInput, SetFocus, INPUT, INPUT_KEYBOARD, KEYEVENTF_KEYUP, VK_LCONTROL, VK_V,
};
use windows::Win32::UI::WindowsAndMessaging::{
  GetForegroundWindow, GetWindowThreadProcessId, SetForegroundWindow,
};

#[tauri::command]
pub fn get_clipboard_data_bytes(format: u32) -> Vec<u8> {
  match open() {
    Err(_e) => {}
    Ok(()) => {
      match size(format) {
        Some(expected_size) => {
          let expected_size = expected_size.into();
          let mut bytes = vec![0u8; expected_size];
          match get(format, &mut bytes) {
            Ok(read_size) => {
              if expected_size > 0 && expected_size == read_size {
                let _ = close();
                return bytes;
              }
            }
            Err(_e) => {}
          }
        }
        None => {}
      }

      let _ = close();
    }
  }

  vec![]
}

#[tauri::command]
pub async fn insert_bytes_data(
  db_instances: State<'_, DbInstances>,
  db_path: &str,
  plugin_id: String,
  representation_data: Vec<u8>,
  representation_metadata: String,
  data: Vec<u8>,
  format_id: i64,
  format: String,
  search_label: Option<String>,
  clipped_at: String,
) -> Result<(), ()> {
  let db_instances = db_instances.0.read().await;
  let db_pool = db_instances.get(db_path).unwrap();
  match db_pool {
    Sqlite(db) => {
      sqlx::query(
        "
      INSERT INTO clips (
        plugin_id,
        representation_data,
        representation_metadata,
        data,
        format_id,
        format,
        search_label,
        clipped_at
      )
      VALUES (?, ?, ?, ?, ?, ?, ?, ?)
      ",
      )
      .bind(plugin_id)
      .bind(representation_data)
      .bind(representation_metadata)
      .bind(data)
      .bind(format_id)
      .bind(format)
      .bind(search_label)
      .bind(clipped_at)
      .execute(db)
      .await
      .unwrap();
    }
  }
  Ok(())
}

#[tauri::command]
pub fn paste_data_bytes(format: u32, bytes: Vec<u8>, target_window_ptr: usize) {
  match open() {
    Err(_e) => {}
    Ok(()) => {
      match set(format, &bytes) {
        Err(_e) => {}
        Ok(()) => unsafe {
          let hwnd = HWND(target_window_ptr as *mut c_void);

          let hwnd_thread = GetWindowThreadProcessId(hwnd, None);
          let current_thread = GetCurrentThreadId();
          if hwnd_thread != current_thread {
            let _ = AttachThreadInput(current_thread, hwnd_thread, true);
          }

          let _ = SetForegroundWindow(hwnd);
          SetFocus(Some(hwnd)).unwrap();

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
        },
      }

      let _ = close();
    }
  }
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
