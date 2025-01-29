use clipboard_win::raw::{close, get, open, size};

#[tauri::command]
pub fn get_clipboard_data_bytes(format: u32) -> Vec<u8> {
  match open() {
    Err(_e) => {}
    Ok(()) => {
      match size(format) {
        Some(expected_size) => {
          let expected_size = expected_size.into();
          let mut bytes = vec![0u8; expected_size];
          match get(format, &mut bytes) {
            Ok(read_size) => {
              if expected_size > 0 && expected_size == read_size {
                let _ = close();
                return bytes;
              }
            }
            Err(_e) => {}
          }
        }
        None => {}
      }

      let _ = close();
    }
  }

  vec![]
}
