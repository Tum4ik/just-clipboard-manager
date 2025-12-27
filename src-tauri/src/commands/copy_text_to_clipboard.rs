use clipboard_win::set_clipboard_string;

#[tauri::command]
pub fn copy_text_to_clipboard(text: &str) -> Result<(), String> {
  let set_result = set_clipboard_string(text);
  if let Err(e) = set_result {
    return Err(format!("Failed to copy text to clipboard. {e}"));
  }

  Ok(())
}
