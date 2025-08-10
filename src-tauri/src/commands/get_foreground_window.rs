use windows::Win32::UI::WindowsAndMessaging::GetForegroundWindow;

#[tauri::command]
pub fn get_foreground_window() -> usize {
  unsafe { GetForegroundWindow().0 as usize }
}
