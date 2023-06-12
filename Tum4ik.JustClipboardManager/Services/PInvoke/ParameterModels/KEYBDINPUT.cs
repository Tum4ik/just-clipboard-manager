namespace Tum4ik.JustClipboardManager.Services.PInvoke.ParameterModels;

/// <summary>
/// Contains information about a simulated keyboard event.
/// </summary>
internal struct KEYBDINPUT
{
  /// <summary>
  /// A virtual-key code. The code must be a value in the range 1 to 254.
  /// If the <see cref="dwFlags"/> member specifies <see cref="KEYEVENT.KEYEVENTF_UNICODE"/>,
  /// <see cref="wVk"/> must be 0.
  /// </summary>
  public ushort wVk;

  /// <summary>
  /// A hardware scan code for the key. If <see cref="dwFlags"/> specifies <see cref="KEYEVENT.KEYEVENTF_UNICODE"/>,
  /// <see cref="wScan"/> specifies a Unicode character which is to be sent to the foreground application.
  /// </summary>
  public ushort wScan;

  /// <summary>
  /// Specifies various aspects of a keystroke. This member can be certain combinations of the values.
  /// </summary>
  public KEYEVENT dwFlags;

  /// <summary>
  /// The time stamp for the event, in milliseconds.
  /// If this parameter is zero, the system will provide its own time stamp.
  /// </summary>
  public uint time;

  /// <summary>
  /// An additional value associated with the keystroke.
  /// Use the GetMessageExtraInfo function to obtain this information.
  /// </summary>
  public nint dwExtraInfo;
}


[Flags]
internal enum KEYEVENT : uint
{
  /// <summary>
  /// If specified, the <see cref="KEYBDINPUT.wScan"/> scan code consists of a sequence of two bytes,
  /// where the first byte has a value of 0xE0.
  /// </summary>
  KEYEVENTF_EXTENDEDKEY = 0x0001,
  /// <summary>
  /// If specified, the key is being released. If not specified, the key is being pressed.
  /// </summary>
  KEYEVENTF_KEYUP = 0x0002,
  /// <summary>
  /// If specified, <see cref="KEYBDINPUT.wScan"/> identifies the key and <see cref="KEYBDINPUT.wVk"/> is ignored.
  /// </summary>
  KEYEVENTF_SCANCODE = 0x0008,
  /// <summary>
  /// If specified, the system synthesizes a VK_PACKET keystroke.
  /// The <see cref="KEYBDINPUT.wVk"/> parameter must be zero.
  /// This flag can only be combined with the <see cref="KEYEVENTF_KEYUP"/> flag.
  /// </summary>
  KEYEVENTF_UNICODE = 0x0004
}
