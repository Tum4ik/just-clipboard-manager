#[tauri::command]
pub fn sentry_capture_info(message: &str) {
  #[cfg(dev)]
  {
    log::info!("{message}");
  }
  sentry::capture_message(message, sentry::Level::Info);
}

#[tauri::command]
pub fn sentry_capture_warning(message: &str) {
  #[cfg(dev)]
  {
    log::warn!("{message}");
  }
  sentry::capture_message(message, sentry::Level::Warning);
}

#[tauri::command]
pub fn sentry_capture_error(message: &str) {
  #[cfg(dev)]
  {
    log::error!("{message}");
  }
  sentry::capture_message(message, sentry::Level::Error);
}
