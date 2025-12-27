#[tauri::command]
pub fn version() -> Option<&'static str> {
  option_env!("CARGO_PKG_VERSION")
}

#[tauri::command]
pub fn authors() -> Option<&'static str> {
  option_env!("CARGO_PKG_AUTHORS")
}
