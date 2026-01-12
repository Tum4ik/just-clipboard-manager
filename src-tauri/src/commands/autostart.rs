use std::path::PathBuf;
use windows::{
  core::{Interface, Result as WinResult, HSTRING, PWSTR},
  Win32::{
    System::Com::{
      CoCreateInstance, CoInitializeEx, CoTaskMemFree, CoUninitialize, IPersistFile,
      CLSCTX_INPROC_SERVER, COINIT_APARTMENTTHREADED,
    },
    UI::Shell::{FOLDERID_Startup, IShellLinkW, SHGetKnownFolderPath, ShellLink, KF_FLAG_DEFAULT},
  },
};

#[tauri::command]
pub fn autostart_is_enabled(app: tauri::AppHandle) -> Result<bool, String> {
  let shortcut_path = shortcut_path(&app)?;
  Ok(shortcut_path.exists())
}

#[tauri::command]
pub fn autostart_enable(app: tauri::AppHandle) -> Result<(), String> {
  let _com_guard = ComGuard::new().map_err(|err| format!("{err}"))?;

  let exe_path = std::env::current_exe().map_err(|err| format!("{err}"))?;
  let shortcut_path = shortcut_path(&app)?;

  unsafe {
    let shell_link: IShellLinkW =
      CoCreateInstance(&ShellLink, None, CLSCTX_INPROC_SERVER).map_err(|err| format!("{err}"))?;
    shell_link
      .SetPath(&HSTRING::from(exe_path.to_string_lossy().as_ref()))
      .map_err(|err| format!("{err}"))?;

    if let Some(parent) = exe_path.parent() {
      shell_link
        .SetWorkingDirectory(&HSTRING::from(parent.to_string_lossy().as_ref()))
        .map_err(|err| format!("{err}"))?;
    }

    let persist: IPersistFile = shell_link.cast().map_err(|err| format!("{err}"))?;
    persist
      .Save(
        &HSTRING::from(shortcut_path.to_string_lossy().as_ref()),
        true,
      )
      .map_err(|err| format!("{err}"))?;
  }

  Ok(())
}

#[tauri::command]
pub fn autostart_disable(app: tauri::AppHandle) -> Result<(), String> {
  let shortcut_path = shortcut_path(&app)?;
  if shortcut_path.exists() {
    std::fs::remove_file(shortcut_path).map_err(|err| format!("{err}"))?;
  }

  Ok(())
}

fn shortcut_path(app: &tauri::AppHandle) -> Result<PathBuf, String> {
  let name = app
    .config()
    .product_name
    .clone()
    .ok_or("productName is missing in tauri.conf.json")?;

  Ok(startup_dir()?.join(format!("{name}.lnk")))
}

fn startup_dir() -> Result<PathBuf, String> {
  unsafe {
    let path: PWSTR = SHGetKnownFolderPath(&FOLDERID_Startup, KF_FLAG_DEFAULT, None)
      .map_err(|err| format!("{err}"))?;

    let result = path
      .to_string()
      .map_err(|err| format!("Invalid UTF-16 startup path. {err}"))?
      .into();

    CoTaskMemFree(Some(path.0 as _));

    Ok(result)
  }
}

// ---------------- RAII COM guard ----------------

struct ComGuard;

impl ComGuard {
  fn new() -> WinResult<Self> {
    unsafe {
      let _ = CoInitializeEx(None, COINIT_APARTMENTTHREADED);
    }
    Ok(Self)
  }
}

impl Drop for ComGuard {
  fn drop(&mut self) {
    unsafe { CoUninitialize() }
  }
}
