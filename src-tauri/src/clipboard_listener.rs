use clipboard_win::formats;
use clipboard_win::raw::{close, format_name_big, get, open, set, EnumFormats};
use std::sync::OnceLock;
use tauri::{App, AppHandle, Emitter, Manager};
use windows::Win32::Foundation::{HWND, LPARAM, LRESULT, WPARAM};
use windows::Win32::System::DataExchange::AddClipboardFormatListener;
use windows::Win32::UI::WindowsAndMessaging::{
  DefWindowProcW, SetWindowLongPtrW, GWLP_WNDPROC, WM_CLIPBOARDUPDATE,
};

static APP_HANDLE: OnceLock<AppHandle> = OnceLock::new();

pub fn clipboard_listener(app: &mut App) -> Result<(), Box<dyn std::error::Error>> {
  APP_HANDLE.set(app.handle().clone()).unwrap();

  let wind = app.get_webview_window("paste-window").unwrap();
  let hwnd = wind.hwnd().unwrap();
  let hwnd = HWND(hwnd.0);

  unsafe {
    AddClipboardFormatListener(hwnd).expect("add failed");

    let prev_proc = SetWindowLongPtrW(hwnd, GWLP_WNDPROC, wnd_proc as _);
    if prev_proc == 0 {
      eprintln!("Failed to set window procedure.");
    }
  }

  Ok(())
}

unsafe extern "system" fn wnd_proc(
  hwnd: HWND,
  msg: u32,
  wparam: WPARAM,
  lparam: LPARAM,
) -> LRESULT {
  match msg {
    WM_CLIPBOARDUPDATE => {
      match open() {
        Err(_e) => {}
        Ok(()) => {
          let available_formats = EnumFormats::new();
          let mut result_formats = Vec::<String>::new();
          for format in available_formats {
            match format_name_big(format) {
              Some(name) => result_formats.push(name),
              None => {}
            }
          }

          match APP_HANDLE.get() {
            Some(app) => {
              let _ = app.emit("clipboard-listener::available-formats", result_formats);
            }
            None => {}
          }

          /* let mut bytes = vec![0u8; 1024];
          match get(formats::CF_UNICODETEXT, &mut bytes) {
            Ok(size) => {
              bytes.truncate(size);
              println!("{bytes:?}");
              let app = APP_HANDLE.get().unwrap();
              app.emit("bytes", bytes);
            }
            Err(_e) => {},
          } */

          let _ = close();
        }
      }
    }
    _ => {}
  }

  // Call the original window procedure
  DefWindowProcW(hwnd, msg, wparam, lparam)
}
