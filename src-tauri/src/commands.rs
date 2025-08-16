pub mod environment;
pub mod extract_and_remove_zip;
pub mod get_foreground_window;
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
      commands::open_main_window::open_main_window,
      commands::environment::environment,
      commands::extract_and_remove_zip::extract_and_remove_zip,

      commands::sentry::sentry_capture_info,
      commands::sentry::sentry_capture_warning,
      commands::sentry::sentry_capture_error,
      commands::sentry::sentry_capture_fatal,
    ]
  };
}
