use windows::Win32::UI::Input::KeyboardAndMouse::{
  RegisterHotKey, UnregisterHotKey, HOT_KEY_MODIFIERS, MOD_ALT, MOD_CONTROL, MOD_SHIFT, MOD_WIN,
  VIRTUAL_KEY, VK_ADD, VK_APPS, VK_BACK, VK_CAPITAL, VK_CONTROL, VK_DECIMAL, VK_DELETE, VK_DIVIDE,
  VK_DOWN, VK_END, VK_ESCAPE, VK_F1, VK_F10, VK_F11, VK_F12, VK_F2, VK_F3, VK_F4, VK_F5, VK_F6,
  VK_F7, VK_F8, VK_F9, VK_HOME, VK_INSERT, VK_LEFT, VK_LWIN, VK_MENU, VK_MULTIPLY, VK_NEXT,
  VK_NUMLOCK, VK_NUMPAD0, VK_NUMPAD1, VK_NUMPAD2, VK_NUMPAD3, VK_NUMPAD4, VK_NUMPAD5, VK_NUMPAD6,
  VK_NUMPAD7, VK_NUMPAD8, VK_NUMPAD9, VK_OEM_1, VK_OEM_2, VK_OEM_3, VK_OEM_4, VK_OEM_5, VK_OEM_6,
  VK_OEM_7, VK_OEM_COMMA, VK_OEM_MINUS, VK_OEM_PERIOD, VK_OEM_PLUS, VK_PAUSE, VK_PRIOR, VK_RETURN,
  VK_RIGHT, VK_RWIN, VK_SCROLL, VK_SHIFT, VK_SPACE, VK_SUBTRACT, VK_TAB, VK_UP,
};

#[tauri::command]
pub fn is_shortcut_registered(
  code: &str,
  has_ctrl: bool,
  has_shift: bool,
  has_alt: bool,
  has_meta: bool,
) -> Result<bool, String> {
  let Some(key) = parse_key(code) else {
    return Err(format!("Unsupported keyboard key: {code}"));
  };

  let mut modifiers = HOT_KEY_MODIFIERS::default();
  if has_ctrl {
    modifiers |= MOD_CONTROL;
  }
  if has_shift {
    modifiers |= MOD_SHIFT;
  }
  if has_alt {
    modifiers |= MOD_ALT;
  }
  if has_meta {
    modifiers |= MOD_WIN;
  }
  unsafe {
    // Use a dummy ID for checking. This ID is only used for the duration of this check.
    // The value must be in the range 0x0000 through 0xBFFF.
    let hotkey_id = 0xBEEF;

    let result = RegisterHotKey(None, hotkey_id, modifiers, key.0 as u32);

    if let Err(_e) = result {
      // Failed, so it is likely registered by another application.
      return Ok(true);
    }

    // Succeeded, so it was not registered. Unregister it immediately.
    let _ = UnregisterHotKey(None, hotkey_id);
    return Ok(false);
  }
}

fn parse_key(code: &str) -> Option<VIRTUAL_KEY> {
  let code = code.to_uppercase().replace("KEY", "").replace("DIGIT", "");
  if code.len() == 1 {
    let character = code.chars().next().unwrap();
    if (character >= 'A' && character <= 'Z') || (character >= '0' && character <= '9') {
      return Some(VIRTUAL_KEY(character as u16));
    }
  }

  match code.as_str() {
    "ESCAPE" => Some(VK_ESCAPE),
    "F1" => Some(VK_F1),
    "F2" => Some(VK_F2),
    "F3" => Some(VK_F3),
    "F4" => Some(VK_F4),
    "F5" => Some(VK_F5),
    "F6" => Some(VK_F6),
    "F7" => Some(VK_F7),
    "F8" => Some(VK_F8),
    "F9" => Some(VK_F9),
    "F10" => Some(VK_F10),
    "F11" => Some(VK_F11),
    "F12" => Some(VK_F12),
    "SCROLLLOCK" => Some(VK_SCROLL),
    "PAUSE" => Some(VK_PAUSE),
    "BACKQUOTE" => Some(VK_OEM_3),
    "MINUS" => Some(VK_OEM_MINUS),
    "EQUAL" => Some(VK_OEM_PLUS),
    "BACKSPACE" => Some(VK_BACK),
    "TAB" => Some(VK_TAB),
    "CAPSLOCK" => Some(VK_CAPITAL),
    "SHIFTLEFT" | "SHIFTRIGHT" => Some(VK_SHIFT),
    "CONTROLLEFT" | "CONTROLRIGHT" => Some(VK_CONTROL),
    "METALEFT" => Some(VK_LWIN),
    "METARIGHT" => Some(VK_RWIN),
    "CONTEXTMENU" => Some(VK_APPS),
    "ALTLEFT" | "ALTRIGHT" => Some(VK_MENU),
    "SPACE" => Some(VK_SPACE),
    "BACKSLASH" => Some(VK_OEM_5),
    "ENTER" | "NUMPADENTER" => Some(VK_RETURN),
    "BRACKETLEFT" => Some(VK_OEM_4),
    "BRACKETRIGHT" => Some(VK_OEM_6),
    "SEMICOLON" => Some(VK_OEM_1),
    "QUOTE" => Some(VK_OEM_7),
    "COMMA" => Some(VK_OEM_COMMA),
    "PERIOD" => Some(VK_OEM_PERIOD),
    "SLASH" => Some(VK_OEM_2),
    "INSERT" => Some(VK_INSERT),
    "DELETE" => Some(VK_DELETE),
    "HOME" => Some(VK_HOME),
    "END" => Some(VK_END),
    "PAGEUP" => Some(VK_PRIOR),
    "PAGEDOWN" => Some(VK_NEXT),
    "ARROWLEFT" => Some(VK_LEFT),
    "ARROWRIGHT" => Some(VK_RIGHT),
    "ARROWUP" => Some(VK_UP),
    "ARROWDOWN" => Some(VK_DOWN),
    "NUMLOCK" => Some(VK_NUMLOCK),
    "NUMPADDIVIDE" => Some(VK_DIVIDE),
    "NUMPADMULTIPLY" => Some(VK_MULTIPLY),
    "NUMPADSUBTRACT" => Some(VK_SUBTRACT),
    "NUMPADADD" => Some(VK_ADD),
    "NUMPADDECIMAL" => Some(VK_DECIMAL),
    "NUMPAD0" => Some(VK_NUMPAD0),
    "NUMPAD1" => Some(VK_NUMPAD1),
    "NUMPAD2" => Some(VK_NUMPAD2),
    "NUMPAD3" => Some(VK_NUMPAD3),
    "NUMPAD4" => Some(VK_NUMPAD4),
    "NUMPAD5" => Some(VK_NUMPAD5),
    "NUMPAD6" => Some(VK_NUMPAD6),
    "NUMPAD7" => Some(VK_NUMPAD7),
    "NUMPAD8" => Some(VK_NUMPAD8),
    "NUMPAD9" => Some(VK_NUMPAD9),
    _ => None,
  }
}
