#[tauri::command]
pub fn sentry_capture_info(message: &str) {
  #[cfg(dev)]
  {
    println!("{message}");
  }
  sentry::capture_message(message, sentry::Level::Info);
}

#[tauri::command]
pub fn sentry_capture_warning(message: &str) {
  #[cfg(dev)]
  {
    println!("{message}");
  }
  sentry::capture_message(message, sentry::Level::Warning);
}

#[tauri::command]
pub fn sentry_capture_error(message: &str) {
  #[cfg(dev)]
  {
    eprintln!("{message}");
  }
  sentry::capture_message(message, sentry::Level::Error);
}
