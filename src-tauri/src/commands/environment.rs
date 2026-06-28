use crate::config::app_config::AppConfiguration;
use tauri::State;

#[tauri::command]
pub fn is_development() -> bool {
  cfg!(dev)
}

#[tauri::command]
pub fn db_connection_string(app_config: State<'_, AppConfiguration>) -> String {
  app_config.database.connection_string.clone()
}
