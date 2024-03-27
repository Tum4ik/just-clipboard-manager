using System.Reflection.Metadata;
using Tum4ik.JustClipboardManager.Services.Theme;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Dwm;
using Windows.Win32.UI.WindowsAndMessaging;

namespace Tum4ik.JustClipboardManager.Helpers;
internal static class WindowHelper
{
  public static void RemoveDefaultTitleBar(nint windowHandle)
  {
    var hwnd = (HWND) windowHandle;
    var windowStyleLong = PInvoke.GetWindowLong(hwnd, WINDOW_LONG_PTR_INDEX.GWL_STYLE);
    windowStyleLong &= ~(int) WINDOW_STYLE.WS_SYSMENU;
    if (nint.Size == 4)
    {
      _ = PInvoke.SetWindowLong(hwnd, WINDOW_LONG_PTR_INDEX.GWL_STYLE, windowStyleLong);
    }
    else
    {
      PInvoke.SetWindowLongPtr(hwnd, WINDOW_LONG_PTR_INDEX.GWL_STYLE, windowStyleLong);
    }
  }


  public static unsafe void ApplyBackdrop(nint windowHandle)
  {
    if (Environment.OSVersion.Version >= new Version("10.0.22523"))
    {
      var backdropType = DWM_SYSTEMBACKDROP_TYPE.DWMSBT_MAINWINDOW;
      PInvoke.DwmSetWindowAttribute(
        (HWND) windowHandle,
        DWMWINDOWATTRIBUTE.DWMWA_SYSTEMBACKDROP_TYPE,
        &backdropType,
        sizeof(DWM_SYSTEMBACKDROP_TYPE)
      );
    }
    else
    {
      var backdropPvAttribute = 1;
      PInvoke.DwmSetWindowAttribute(
        (HWND) windowHandle,
        (DWMWINDOWATTRIBUTE) 1029,
        &backdropPvAttribute,
        sizeof(int)
      );
    }
  }


  public static unsafe void ApplyTheme(nint windowHandle, ThemeType themeType)
  {
    var dwAttribute = Environment.OSVersion.Version >= new Version("10.0.22523")
      ? DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE
      : (DWMWINDOWATTRIBUTE) 19;
    var enableDark = themeType == ThemeType.Dark ? 0x1 : 0x0;
    PInvoke.DwmSetWindowAttribute((HWND) windowHandle, dwAttribute, &enableDark, sizeof(int));
  }
}
