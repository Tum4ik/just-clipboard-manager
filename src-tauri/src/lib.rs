mod clipboard_listener;
mod commands;
mod helpers;
mod migrations;

use clipboard_listener::clipboard_listener;
use config::Config;
use log::LevelFilter;
use tauri_plugin_log::fern::colors::ColoredLevelConfig;

#[cfg_attr(mobile, tauri::mobile_entry_point)]
pub fn run(config: Config) {
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
          config
            .get_string("database.connection-string")
            .expect("'database.connection-string' not found in config")
            .as_str(),
          migrations::migrations(),
        )
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
    .build(tauri::generate_context!())
    .expect("error while building tauri application")
    .run(|_, event| match event {
      tauri::RunEvent::Ready => {
        sentry::start_session();
      }
      tauri::RunEvent::Exit => {
        sentry::end_session();
      }
      _ => {}
    });
}
