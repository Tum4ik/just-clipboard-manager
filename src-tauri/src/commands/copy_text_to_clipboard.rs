use clipboard_win::raw::{close, open, set_string};

#[tauri::command]
pub fn copy_text_to_clipboard(text: &str) -> Result<(), String> {
  if let Err(e) = open() {
    return Err(format!("Can't open clipboard. {e}"));
  }

  let set_result = set_string(text);
  let _ = close();

  if let Err(e) = set_result {
    return Err(format!("Can't set text to clipboard. {e}"));
  }

  Ok(())
}
