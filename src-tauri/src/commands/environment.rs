use config::Config;
use tauri::State;

#[tauri::command]
pub fn environment(config: State<'_, Config>) -> String {
  config.get_string("environment").unwrap()
}
