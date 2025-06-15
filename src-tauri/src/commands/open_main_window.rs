use std::path::{ PathBuf};
use tauri::{ WebviewUrl};

#[tauri::command]
pub fn open_main_window(app: tauri::AppHandle, section: Option<&str>) {
  let handle = app.clone();
  match handle
    .config()
    .app
    .windows
    .iter()
    .find(|c| c.label == "main-window")
    .cloned()
  {
    Some(mut config) => {
      if section.is_some() {
        let new_url = config.url.to_string() + "/" + section.unwrap();
        config.url = WebviewUrl::App(PathBuf::from(new_url));
      }
      std::thread::spawn(move || {
        tauri::WebviewWindowBuilder::from_config(&handle, &config)
          .unwrap()
          .build()
          .unwrap();
      });
    }
    None => {
      sentry::capture_message("Config for main window is not found", sentry::Level::Fatal);
    }
  }
}
