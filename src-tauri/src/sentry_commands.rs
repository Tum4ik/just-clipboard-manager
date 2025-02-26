#[tauri::command]
pub fn sentry_capture_info(message: &str) {
  capture_message(message, sentry::Level::Info);
}

#[tauri::command]
pub fn sentry_capture_warning(message: &str) {
  capture_message(message, sentry::Level::Warning);
}

#[tauri::command]
pub fn sentry_capture_error(message: &str) {
  capture_message(message, sentry::Level::Error);
}

fn capture_message(message: &str, level: sentry::Level) {
  #[cfg(dev)]
  {
    println!("{message}");
  }

  sentry::capture_message(message, level);
}
