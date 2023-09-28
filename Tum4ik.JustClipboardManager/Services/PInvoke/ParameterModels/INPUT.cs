using System.Runtime.InteropServices;

namespace Tum4ik.JustClipboardManager.Services.PInvoke.ParameterModels;

#pragma warning disable S101 // Types should be named in PascalCase
#pragma warning disable IDE1006 // Naming Styles

/// <summary>
/// Used by SendInput to store information for synthesizing input events
/// such as keystrokes, mouse movement, and mouse clicks.
/// </summary>
internal struct INPUT
{
  /// <summary>
  /// The type of the input event. This member can be one of the values.
  /// </summary>
  public INPUTTYPE type;

  public MOUSEKEYBDHARDWAREINPUT data;
}


internal enum INPUTTYPE : uint
{
  /// <summary>
  /// The event is a mouse event.
  /// </summary>
  INPUT_MOUSE = 0,
  /// <summary>
  /// The event is a keyboard event.
  /// </summary>
  INPUT_KEYBOARD = 1,
  /// <summary>
  /// The event is a hardware event.
  /// </summary>
  INPUT_HARDWARE = 2
}


[StructLayout(LayoutKind.Explicit)]
internal struct MOUSEKEYBDHARDWAREINPUT
{
  /// <summary>
  /// The information about a simulated mouse event.
  /// </summary>
  [FieldOffset(0)]
  public MOUSEINPUT mi;

  /// <summary>
  /// The information about a simulated keyboard event.
  /// </summary>
  [FieldOffset(0)]
  public KEYBDINPUT ki;

  /// <summary>
  /// The information about a simulated hardware event.
  /// </summary>
  [FieldOffset(0)]
  public HARDWAREINPUT hi;
}
