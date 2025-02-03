mod clipboard_listener;
mod commands;
mod migrations;

use clipboard_listener::clipboard_listener;

#[cfg_attr(mobile, tauri::mobile_entry_point)]
pub fn run() {
  tauri::Builder::default()
    .plugin(
      tauri_plugin_sql::Builder::new()
        .add_migrations("sqlite:jcm-database.db", migrations::migrations())
        .build(),
    )
    .plugin(tauri_plugin_opener::init())
    .plugin(tauri_plugin_fs::init())
    .plugin(tauri_plugin_process::init())
    .plugin(tauri_plugin_global_shortcut::Builder::new().build())
    .plugin(tauri_plugin_store::Builder::new().build())
    .setup(clipboard_listener)
    .invoke_handler(tauri::generate_handler![commands::get_clipboard_data_bytes])
    .run(tauri::generate_context!())
    .expect("error while running tauri application");
}
