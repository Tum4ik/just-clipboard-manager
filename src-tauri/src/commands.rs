pub mod autostart;
pub mod copy_text_to_clipboard;
pub mod environment;
pub mod extract_and_remove_zip;
pub mod get_caret_position;
pub mod get_foreground_window;
pub mod info;
pub mod is_shortcut_registered;
pub mod open_main_window;
pub mod paste_clip;
pub mod save_data_objects_and_get_representation_bytes;
pub mod sentry;
pub mod update_clip;

#[macro_export]
macro_rules! all_commands {
  () => {
    tauri::generate_handler![
      commands::save_data_objects_and_get_representation_bytes::save_data_objects_and_get_representation_bytes,
      commands::update_clip::update_clip,
      commands::paste_clip::paste_clip,
      commands::get_foreground_window::get_foreground_window,
      commands::get_caret_position::get_caret_position,
      commands::open_main_window::open_main_window,
      commands::extract_and_remove_zip::extract_and_remove_zip,
      commands::is_shortcut_registered::is_shortcut_registered,
      commands::copy_text_to_clipboard::copy_text_to_clipboard,

      commands::environment::environment,
      commands::environment::db_connection_string,

      commands::info::info_product_name,
      commands::info::info_version,
      commands::info::info_authors,

      commands::autostart::autostart_is_enabled,
      commands::autostart::autostart_enable,
      commands::autostart::autostart_disable,

      commands::sentry::sentry_capture_info,
      commands::sentry::sentry_capture_warning,
      commands::sentry::sentry_capture_error,
      commands::sentry::sentry_capture_fatal,
    ]
  };
}
