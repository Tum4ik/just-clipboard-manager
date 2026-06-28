mod commands;
pub mod config;
mod helpers;
mod migrations;
mod setup;

use crate::config::app_config::AppConfiguration;
use crate::setup::clipboard_listener::setup_clipboard_listener;
use log::LevelFilter;
use tauri_plugin_log::fern::colors::ColoredLevelConfig;

#[cfg_attr(mobile, tauri::mobile_entry_point)]
pub fn run(app_config: AppConfiguration) {
  tauri::Builder::default()
    .plugin(tauri_plugin_updater::Builder::new().build())
    .plugin(tauri_plugin_single_instance::init(|_app, _args, _cwd| {}))
    .plugin(tauri_plugin_positioner::init())
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
        .add_migrations(
          &app_config.database.connection_string,
          migrations::migrations(),
        )
        .build(),
    )
    .plugin(tauri_plugin_opener::init())
    .plugin(tauri_plugin_fs::init())
    .plugin(tauri_plugin_process::init())
    .plugin(tauri_plugin_global_shortcut::Builder::new().build())
    .plugin(tauri_plugin_store::Builder::new().build())
    .manage(app_config)
    .setup(|app| {
      setup_clipboard_listener(app)?;
      Ok(())
    })
    .invoke_handler(all_commands!())
    .build(tauri::generate_context!())
    .expect("error while building tauri application")
    .run(|_, event| match event {
      tauri::RunEvent::Ready => {
        sentry::start_session();
        sentry::capture_message("Application started", sentry::Level::Info);
      }
      tauri::RunEvent::Exit => {
        sentry::end_session();
        sentry::capture_message("Application closed", sentry::Level::Info);
      }
      _ => {}
    });
}
