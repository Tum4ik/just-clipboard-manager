// Learn more about Tauri commands at https://tauri.app/develop/calling-rust/
#[tauri::command]
fn greet(name: &str) -> String {
  format!("Hello, {}! You've been greeted from Rust!", name)
}

use libloading::Error;
use libloading::Library;

#[cfg_attr(mobile, tauri::mobile_entry_point)]
pub fn run() {
  unsafe {
    println!("=============== start unsafe");
    let lib = Library::new("text_plugin.dll").expect("lib error");

    // let test_func: libloading::Symbol<unsafe extern "C" fn() -> String> = lib.get(b"test");
    let test_func: Result<libloading::Symbol<unsafe extern "C" fn() -> u32>, Error> =
      lib.get(b"test");
    match test_func {
      Ok(func) => {
        let str = func();
        println!("{str}");
      }
      Err(e) => {
        println!("=============== func error");
        println!("{e}");
        // GetProcAddress failed
      }
    }
  }

  tauri::Builder::default()
    .plugin(tauri_plugin_opener::init())
    .plugin(tauri_plugin_fs::init())
    .invoke_handler(tauri::generate_handler![greet])
    .run(tauri::generate_context!())
    .expect("error while running tauri application");
}
