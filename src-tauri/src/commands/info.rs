#[tauri::command]
pub fn info_product_name(app: tauri::AppHandle) -> Option<String> {
  app.config().product_name.clone()
}

#[tauri::command]
pub fn info_version() -> Option<&'static str> {
  option_env!("CARGO_PKG_VERSION")
}

#[tauri::command]
pub fn info_authors() -> Option<&'static str> {
  option_env!("CARGO_PKG_AUTHORS")
}
