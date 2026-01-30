use crate::helpers::database::get_sqlite_db;
use clipboard_win::raw::{close, open, set, set_without_clear};
use clipboard_win::SysResult;
use config::Config;
use sqlx::Row;
use std::ffi::c_void;
use tauri::State;
use tauri_plugin_sql::DbInstances;
use windows::Win32::Foundation::HWND;
use windows::Win32::System::Threading::{AttachThreadInput, GetCurrentThreadId};
use windows::Win32::UI::Input::KeyboardAndMouse::{
  SendInput, INPUT, INPUT_KEYBOARD, KEYEVENTF_KEYUP, VK_LCONTROL, VK_V,
};
use windows::Win32::UI::WindowsAndMessaging::{GetWindowThreadProcessId, SetForegroundWindow};

#[tauri::command]
pub async fn paste_clip(
  config: State<'_, Config>,
  db_instances: State<'_, DbInstances>,
  clip_id: i64,
  target_window_ptr: usize,
) -> Result<(), String> {
  let db = get_sqlite_db(config, db_instances)
    .await
    .map_err(|e| format!("Can't get SQLite DB: {}", e))?;
  let query_result = sqlx::query(
    "
    SELECT format_id, data
    FROM data_objects
    WHERE clip_id = ?
    ",
  )
  .bind(clip_id)
  .fetch_all(&db)
  .await
  .map_err(|e| e.to_string())?;

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
