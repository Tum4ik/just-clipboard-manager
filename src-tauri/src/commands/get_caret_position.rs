use serde::Serialize;
use windows::Win32::System::Variant::VARIANT;
use windows::{
  core::Interface,
  Win32::UI::{
    Accessibility::{AccessibleObjectFromWindow, IAccessible},
    WindowsAndMessaging::{GetForegroundWindow, OBJID_CARET},
  },
};

#[derive(Serialize)]
pub struct CaretPosition {
  pub x: i32,
  pub y: i32,
}

#[tauri::command]
pub fn get_caret_position() -> Result<CaretPosition, String> {
  unsafe {
    let foreground_window = GetForegroundWindow();

    // Get the IAccessible interface for the caret
    let mut accessible: Option<IAccessible> = None;
    let result = AccessibleObjectFromWindow(
      foreground_window,
      OBJID_CARET.0 as u32,
      &IAccessible::IID,
      &mut accessible as *mut _ as *mut _,
    );

    if let Err(e) = result {
      return Err(format!("Failed to get accessible object: {e}"));
    }

    if let Some(accessible) = accessible {
      let mut left: i32 = 0;
      let mut top: i32 = 0;
      let mut width: i32 = 0;
      let mut height: i32 = 0;

      // Get the caret location
      if let Err(e) = accessible.accLocation(
        &mut left,
        &mut top,
        &mut width,
        &mut height,
        &VARIANT::from(0),
      ) {
        return Err(format!("Can't access caret location: {e}"));
      }

      return Ok(CaretPosition { x: left, y: top });
    }

    Err("Failed to get caret position".into())
  }
}
