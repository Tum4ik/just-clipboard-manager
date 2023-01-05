namespace Tum4ik.JustClipboardManager.Services.PInvoke.ParameterModels;
internal enum MonitorOptions : uint
{
  /// <summary>
  /// Returns <see cref="null"/>.
  /// </summary>
  MONITOR_DEFAULTTONULL = 0x00000000,
  /// <summary>
  /// Returns a handle to the primary display monitor.
  /// </summary>
  MONITOR_DEFAULTTOPRIMARY = 0x00000001,
  /// <summary>
  /// Returns a handle to the display monitor that is nearest to the window.
  /// </summary>
  MONITOR_DEFAULTTONEAREST = 0x00000002
}
