namespace Tum4ik.JustClipboardManager.Services.PInvoke.ParameterModels;

#pragma warning disable S101 // Types should be named in PascalCase
#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable CA1712 // Do not prefix enum values with type name

/// <summary>
/// Contains information about a simulated mouse event.
/// </summary>
internal struct MOUSEINPUT
{
  /// <summary>
  /// The absolute position of the mouse, or the amount of motion since the last mouse event was generated,
  /// depending on the value of the <see cref="dwFlags"/> member.
  /// Absolute data is specified as the x coordinate of the mouse;
  /// relative data is specified as the number of pixels moved.
  /// </summary>
  public int dx;

  /// <summary>
  /// The absolute position of the mouse, or the amount of motion since the last mouse event was generated,
  /// depending on the value of the <see cref="dwFlags"/> member.
  /// Absolute data is specified as the y coordinate of the mouse;
  /// relative data is specified as the number of pixels moved.
  /// </summary>
  public int dy;

  /// <summary>
  /// If <see cref="dwFlags"/> contains <see cref="MOUSEEVENT.MOUSEEVENTF_WHEEL"/>,
  /// then mouseData specifies the amount of wheel movement.
  /// A positive value indicates that the wheel was rotated forward, away from the user;
  /// a negative value indicates that the wheel was rotated backward, toward the user.
  /// One wheel click is defined as WHEEL_DELTA, which is 120.
  /// <br/><br/>
  /// If dwFlags does not contain <see cref="MOUSEEVENT.MOUSEEVENTF_WHEEL"/>,
  /// <see cref="MOUSEEVENT.MOUSEEVENTF_XDOWN"/>, or <see cref="MOUSEEVENT.MOUSEEVENTF_XUP"/>,
  /// then mouseData should be zero.
  /// <br/><br/>
  /// If <see cref="dwFlags"/> contains <see cref="MOUSEEVENT.MOUSEEVENTF_XDOWN"/> or
  /// <see cref="MOUSEEVENT.MOUSEEVENTF_XUP"/>, then mouseData specifies which X buttons were pressed or released.
  /// This value may be any combination of the following flags:<br/>
  /// XBUTTON1 | 0x0001 | Set if the first X button is pressed or released.<br/>
  /// XBUTTON2 | 0x0002 | Set if the second X button is pressed or released.
  /// </summary>
  public uint mouseData;

  /// <summary>
  /// A set of bit flags that specify various aspects of mouse motion and button clicks.
  /// The bits in this member can be any reasonable combination of the values.
  /// </summary>
  public MOUSEEVENT dwFlags;

  /// <summary>
  /// The time stamp for the event, in milliseconds. If this parameter is 0, the system will provide its own time stamp.
  /// </summary>
  public uint time;

  /// <summary>
  /// An additional value associated with the mouse event.
  /// An application calls GetMessageExtraInfo to obtain this extra information.
  /// </summary>
  public nint dwExtraInfo;
}


[Flags]
internal enum MOUSEEVENT : uint
{
  /// <summary>
  /// Movement occurred.
  /// </summary>
  MOUSEEVENTF_MOVE = 0x0001,
  /// <summary>
  /// The left button was pressed.
  /// </summary>
  MOUSEEVENTF_LEFTDOWN = 0x0002,
  /// <summary>
  /// The left button was released.
  /// </summary>
  MOUSEEVENTF_LEFTUP = 0x0004,
  /// <summary>
  /// The right button was pressed.
  /// </summary>
  MOUSEEVENTF_RIGHTDOWN = 0x0008,
  /// <summary>
  /// The right button was released.
  /// </summary>
  MOUSEEVENTF_RIGHTUP = 0x0010,
  /// <summary>
  /// The middle button was pressed.
  /// </summary>
  MOUSEEVENTF_MIDDLEDOWN = 0x0020,
  /// <summary>
  /// The middle button was released.
  /// </summary>
  MOUSEEVENTF_MIDDLEUP = 0x0040,
  /// <summary>
  /// An X button was pressed.
  /// </summary>
  MOUSEEVENTF_XDOWN = 0x0080,
  /// <summary>
  /// An X button was released.
  /// </summary>
  MOUSEEVENTF_XUP = 0x0100,
  /// <summary>
  /// The wheel was moved, if the mouse has a wheel.
  /// The amount of movement is specified in <see cref="MOUSEINPUT.mouseData"/>.
  /// </summary>
  MOUSEEVENTF_WHEEL = 0x0800,
  /// <summary>
  /// The wheel was moved horizontally, if the mouse has a wheel.
  /// The amount of movement is specified in <see cref="MOUSEINPUT.mouseData"/>.
  /// </summary>
  MOUSEEVENTF_HWHEEL = 0x1000,
  /// <summary>
  /// The WM_MOUSEMOVE messages will not be coalesced. The default behavior is to coalesce WM_MOUSEMOVE messages.
  /// </summary>
  MOUSEEVENTF_MOVE_NOCOALESCE = 0x2000,
  /// <summary>
  /// Maps coordinates to the entire desktop. Must be used with <see cref="MOUSEEVENTF_ABSOLUTE"/>.
  /// </summary>
  MOUSEEVENTF_VIRTUALDESK = 0x4000,
  /// <summary>
  /// The <see cref="MOUSEINPUT.dx"/> and <see cref="MOUSEINPUT.dy"/> members contain normalized absolute coordinates.
  /// If the flag is not set, <see cref="MOUSEINPUT.dx"/> and <see cref="MOUSEINPUT.dy"/> contain relative data
  /// (the change in position since the last reported position). This flag can be set, or not set,
  /// regardless of what kind of mouse or other pointing device, if any, is connected to the system.
  /// </summary>
  MOUSEEVENTF_ABSOLUTE = 0x8000
}
