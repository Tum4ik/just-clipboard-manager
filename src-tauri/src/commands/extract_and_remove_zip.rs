use std::fs::{remove_file, File};
use std::path::Path;
use zip::ZipArchive;

#[tauri::command]
pub fn extract_and_remove_zip(zip_file_path: String, plugin_extraction_folder: String) {
  let file_path = Path::new(&zip_file_path);
  let file = File::open(file_path).unwrap();
  let mut archive = ZipArchive::new(file).unwrap();
  archive
    .extract(Path::new(&plugin_extraction_folder))
    .unwrap();
  remove_file(file_path).unwrap();
}
