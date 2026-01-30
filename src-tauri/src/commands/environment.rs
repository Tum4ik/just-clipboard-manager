use config::Config;
use tauri::State;

#[tauri::command]
pub fn environment(config: State<'_, Config>) -> String {
  config
    .get_string("environment")
    .expect("'environment' key not found in config")
}

#[tauri::command]
pub fn db_connection_string(config: State<'_, Config>) -> String {
  config
    .get_string("database.connection-string")
    .expect("'database.connection-string' key not found in config")
}
