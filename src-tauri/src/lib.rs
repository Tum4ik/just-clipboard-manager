mod clipboard_listener;
mod commands;
mod migrations;
mod sentry_commands;

use clipboard_listener::clipboard_listener;
use log::LevelFilter;
use tauri_plugin_log::fern::colors::ColoredLevelConfig;

#[cfg_attr(mobile, tauri::mobile_entry_point)]
pub fn run() {
  tauri::Builder::default()
    .plugin(tauri_plugin_http::init())
    .plugin(
      tauri_plugin_log::Builder::new()
        .level(LevelFilter::Info)
        .with_colors(ColoredLevelConfig::default())
        .build(),
    )
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
    .invoke_handler(tauri::generate_handler![
      commands::get_clipboard_data_bytes,
      commands::insert_bytes_data,
      commands::paste_data_bytes,
      commands::get_foreground_window,
      commands::open_main_window,
      sentry_commands::sentry_capture_info,
      sentry_commands::sentry_capture_warning,
      sentry_commands::sentry_capture_error,
      sentry_commands::sentry_capture_fatal,
    ])
    .run(tauri::generate_context!())
    .expect("error while running tauri application");
}
