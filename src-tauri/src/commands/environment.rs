use config::Config;
use tauri::State;

#[tauri::command]
pub fn environment(config: State<'_, Config>) -> String {
  config.get_string("environment").unwrap()
}

#[tauri::command]
pub fn db_connection_string(config: State<'_, Config>) -> String {
  config.get_string("database.connection-string").unwrap()
}
