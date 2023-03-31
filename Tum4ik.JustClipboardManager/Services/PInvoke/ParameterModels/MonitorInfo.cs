using System.Runtime.InteropServices;

namespace Tum4ik.JustClipboardManager.Services.PInvoke.ParameterModels;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
internal struct MonitorInfo
{
  /// <summary>
  /// The size, in bytes, of the structure. Set this member to sizeof(MONITORINFOEX) (72) before calling the GetMonitorInfo function.
  /// Doing so lets the function determine the type of structure you are passing to it.
  /// </summary>
  public int Size { get; set; }

  /// <summary>
  /// A RECT structure that specifies the display monitor rectangle, expressed in virtual-screen coordinates.
  /// Note that if the monitor is not the primary display monitor, some of the rectangle's coordinates may be negative values.
  /// </summary>
  public RectStruct Monitor { get; set; }

  /// <summary>
  /// A RECT structure that specifies the work area rectangle of the display monitor that can be used by applications,
  /// expressed in virtual-screen coordinates. Windows uses this rectangle to maximize an application on the monitor.
  /// The rest of the area in rcMonitor contains system windows such as the task bar and side bars.
  /// Note that if the monitor is not the primary display monitor, some of the rectangle's coordinates may be negative values.
  /// </summary>
  public RectStruct WorkArea { get; set; }

  /// <summary>
  /// The attributes of the display monitor.
  ///
  /// This member can be the following value:
  ///   1 : MONITORINFOF_PRIMARY
  /// </summary>
  public uint Flags { get; set; }
}
