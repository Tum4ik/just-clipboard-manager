using Windows.Win32;
using Windows.Win32.Graphics.Gdi;
using Windows.Win32.UI.HiDpi;

namespace Tum4ik.JustClipboardManager.Services.PInvokeWrappers;

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
  bool GetDpiForMonitor(nint hmonitor, MONITOR_DPI_TYPE dpiType, out uint dpiX, out uint dpiY)
  {
    var result = PInvoke.GetDpiForMonitor((HMONITOR)hmonitor, dpiType, out dpiX, out dpiY);
    return result.Succeeded;
  }
}


internal class SHCoreDllService : ISHCoreDllService { }
