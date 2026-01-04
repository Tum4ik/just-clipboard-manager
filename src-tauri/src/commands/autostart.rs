use std::path::PathBuf;
use windows::{
  core::{Interface, HSTRING, PWSTR},
  Win32::{
    System::Com::{
      CoCreateInstance, CoInitializeEx, CoUninitialize, IPersistFile, CLSCTX_INPROC_SERVER,
      COINIT_APARTMENTTHREADED,
    },
    UI::Shell::{FOLDERID_Startup, IShellLinkW, SHGetKnownFolderPath, ShellLink, KF_FLAG_DEFAULT},
  },
};

#[tauri::command]
pub fn autostart_is_enabled(app: tauri::AppHandle) -> bool {
  let app_name = app.config().product_name.clone().unwrap();
  let shortcut_path = startup_dir().join(format!("{app_name}.lnk"));
  shortcut_path.exists()
}

#[tauri::command]
pub fn autostart_enable(app: tauri::AppHandle) {
  let app_name = app.config().product_name.clone().unwrap();
  unsafe {
    let _ = CoInitializeEx(None, COINIT_APARTMENTTHREADED);

    let exe_path = std::env::current_exe().unwrap();
    let shortcut_path = startup_dir().join(format!("{app_name}.lnk"));

    let shell_link: IShellLinkW = CoCreateInstance(&ShellLink, None, CLSCTX_INPROC_SERVER).unwrap();
    let _ = shell_link.SetPath(&HSTRING::from(exe_path.to_string_lossy().as_ref()));
    let _ = shell_link.SetWorkingDirectory(&HSTRING::from(
      exe_path.parent().unwrap().to_string_lossy().as_ref(),
    ));

    let persist: IPersistFile = shell_link.cast().unwrap();
    let _ = persist.Save(
      &HSTRING::from(shortcut_path.to_string_lossy().as_ref()),
      true,
    );

    CoUninitialize();
  }
}

#[tauri::command]
pub fn autostart_disable(app: tauri::AppHandle) {
  let app_name = app.config().product_name.clone().unwrap();
  let shortcut_path = startup_dir().join(format!("{app_name}.lnk"));
  if shortcut_path.exists() {
    let _ = std::fs::remove_file(shortcut_path);
  }
}

fn startup_dir() -> PathBuf {
  unsafe {
    let path: PWSTR = SHGetKnownFolderPath(&FOLDERID_Startup, KF_FLAG_DEFAULT, None).unwrap();
    PathBuf::from(path.to_string().unwrap())
  }
}
