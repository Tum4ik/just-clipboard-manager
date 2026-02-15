use clipboard_win::raw::{close, format_name_big, open, EnumFormats};
use std::collections::HashMap;
use std::sync::OnceLock;
use tauri::{App, AppHandle, Emitter, Url, WebviewUrl, WebviewWindowBuilder};
use windows::Win32::Foundation::{HWND, LPARAM, LRESULT, WPARAM};
use windows::Win32::System::DataExchange::AddClipboardFormatListener;
use windows::Win32::UI::WindowsAndMessaging::{
  DefWindowProcW, SetWindowLongPtrW, GWLP_WNDPROC, WM_CLIPBOARDUPDATE,
};

static APP_HANDLE: OnceLock<AppHandle> = OnceLock::new();

pub fn clipboard_listener(app: &mut App) -> Result<(), Box<dyn std::error::Error>> {
  APP_HANDLE.set(app.handle().clone()).unwrap();

  let wind_builder = WebviewWindowBuilder::new(
    app,
    "hidden-window-for-clipboard-listener",
    // We need that app://empty to prevent useless bootstrapping of Angular app
    WebviewUrl::CustomProtocol(Url::parse("app://empty").unwrap()),
  );
  let wind = wind_builder.visible(false).build().unwrap();
  let hwnd = wind.hwnd().unwrap();
  let hwnd = HWND(hwnd.0);

  unsafe {
    AddClipboardFormatListener(hwnd).expect("failed to add clipboard format listener");

    let prev_proc = SetWindowLongPtrW(hwnd, GWLP_WNDPROC, wnd_proc as *const () as _);
    if prev_proc == 0 {
      panic!("Failed to set window procedure.");
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
    WM_CLIPBOARDUPDATE => match open() {
      Err(_e) => {}
      Ok(()) => {
        let available_formats = EnumFormats::new();
        let mut result_formats = HashMap::<String, u32>::new();
        for format in available_formats {
          match format_name_big(format) {
            Some(name) => {
              result_formats.insert(name, format);
            }
            None => {}
          }
        }

        match APP_HANDLE.get() {
          Some(app) => {
            let _ = app.emit("clipboard-listener::available-formats", result_formats);
          }
          None => {}
        }

        let _ = close();
      }
    },
    _ => {}
  }

  // Call the original window procedure
  DefWindowProcW(hwnd, msg, wparam, lparam)
}
