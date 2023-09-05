using System.Runtime.InteropServices;
using Tum4ik.JustClipboardManager.Services.PInvoke.ParameterModels;

namespace Tum4ik.JustClipboardManager.Services.PInvoke;

internal interface ISHCoreDllService
{
  /// <summary>
  /// Queries the dots per inch (dpi) of a display.
  /// </summary>
  /// <param name="hmonitor">Handle of the monitor being queried.</param>
  /// <param name="dpiType">The type of DPI being queried.</param>
  /// <param name="dpiX">
  /// The value of the DPI along the X axis. This value always refers to the horizontal edge,
  /// even when the screen is rotated.
  /// </param>
  /// <param name="dpiY">
  /// The value of the DPI along the Y axis. This value always refers to the vertical edge,
  /// even when the screen is rotated.
  /// </param>
  /// <returns>True if operation succeeds, otherwise - false.</returns>
  bool GetDpiForMonitor(nint hmonitor, MonitorDpiType dpiType, out uint dpiX, out uint dpiY)
  {
    var result = _GetDpiForMonitor(hmonitor, dpiType, out dpiX, out dpiY);
    return result == 0;
  }


  [DllImport("SHCore.dll", EntryPoint = "GetDpiForMonitor")]
  [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
  private static extern int _GetDpiForMonitor(nint hmonitor, MonitorDpiType dpiType, out uint dpiX, out uint dpiY);
}


internal class SHCoreDllService : ISHCoreDllService { }
