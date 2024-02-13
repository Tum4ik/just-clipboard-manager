using Windows.Win32;
using Windows.Win32.Graphics.Gdi;
using Windows.Win32.UI.HiDpi;

namespace Tum4ik.JustClipboardManager.Services.PInvokeWrappers;

internal interface ISHCoreDllService
{
  /// <inheritdoc cref="PInvoke.GetDpiForMonitor(HMONITOR, MONITOR_DPI_TYPE, out uint, out uint)"/>
  bool GetDpiForMonitor(nint hmonitor, MONITOR_DPI_TYPE dpiType, out uint dpiX, out uint dpiY)
  {
    var result = PInvoke.GetDpiForMonitor((HMONITOR)hmonitor, dpiType, out dpiX, out dpiY);
    return result.Succeeded;
  }
}


internal class SHCoreDllService : ISHCoreDllService { }
