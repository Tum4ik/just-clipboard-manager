mod clipboard_listener;
mod commands;
mod constants;
mod migrations;

use clipboard_listener::clipboard_listener;
use config::Config;
use constants::*;
use log::LevelFilter;
use tauri_plugin_log::fern::colors::ColoredLevelConfig;

#[cfg_attr(mobile, tauri::mobile_entry_point)]
pub fn run(config: Config) {
  tauri::Builder::default()
    .plugin(tauri_plugin_upload::init())
    .plugin(tauri_plugin_http::init())
    .plugin(
      tauri_plugin_log::Builder::new()
        .level(LevelFilter::Info)
        .with_colors(ColoredLevelConfig::default())
        .build(),
    )
    .plugin(
      tauri_plugin_sql::Builder::new()
        .add_migrations(DB_PATH, migrations::migrations())
        .build(),
    )
    .plugin(tauri_plugin_opener::init())
    .plugin(tauri_plugin_fs::init())
    .plugin(tauri_plugin_process::init())
    .plugin(tauri_plugin_global_shortcut::Builder::new().build())
    .plugin(tauri_plugin_store::Builder::new().build())
    .manage(config)
    .setup(clipboard_listener)
    .invoke_handler(all_commands!())
    .run(tauri::generate_context!())
    .expect("error while running tauri application");
}
