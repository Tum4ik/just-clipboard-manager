use std::path::PathBuf;
use tauri::{Manager, WebviewUrl};

#[tauri::command]
pub fn open_main_window(
  app: tauri::AppHandle,
  top_level_tab_id: Option<&str>,
  nested_level_tab_id: Option<&str>,
) -> Result<(), String> {
  let handle = app.clone();
  match handle
    .config()
    .app
    .windows
    .iter()
    .find(|c| c.label == "main-window")
    .cloned()
  {
    None => {
      let error = "Config for main window is not found";
      sentry::capture_message(error, sentry::Level::Fatal);
      return Err(error.to_string());
    }
    Some(mut config) => {
      let mut new_url = config.url.to_string();
      if let Some(top_level_tab_id) = top_level_tab_id {
        new_url.push_str("?topLevelTabId=");
        new_url.push_str(top_level_tab_id);

        if let Some(nested_level_tab_id) = nested_level_tab_id {
          new_url.push_str("&nestedLevelTabId=");
          new_url.push_str(nested_level_tab_id);
        }
      }

      config.url = WebviewUrl::App(PathBuf::from(new_url.to_string()));

      if let Some(existing_window) = app.get_webview_window("main-window") {
        let _ = existing_window.set_focus();
        return Ok(());
      }
      std::thread::spawn(move || {
        let _ = tauri::WebviewWindowBuilder::from_config(&handle, &config)
          .and_then(|builder| builder.build())
          .and_then(|window| window.set_focus())
          .map_err(|e| sentry::capture_error(&e));
      });
    }
  }
  Ok(())
}
