using System.Drawing;
using Accessibility;
using Tum4ik.JustClipboardManager.Services.PInvokeWrappers;
using Tum4ik.JustClipboardManager.Views;
using Windows.Win32.Graphics.Gdi;
using Windows.Win32.UI.HiDpi;
using Windows.Win32.UI.WindowsAndMessaging;

namespace Tum4ik.JustClipboardManager.Services;
internal class PasteWindowService : IPasteWindowService
{
  private readonly PasteWindow _pasteWindow;
  private readonly IUser32DllService _user32Dll;
  private readonly IOleaccDllService _oleaccDllService;
  private readonly ISHCoreDllService _shCoreDll;
  private readonly ISettingsService _settingsService;

  public PasteWindowService(PasteWindow pasteWindow,
                            IUser32DllService user32Dll,
                            IOleaccDllService oleaccDllService,
                            ISHCoreDllService shCoreDll,
                            ISettingsService settingsService)
  {
    _pasteWindow = pasteWindow;
    _user32Dll = user32Dll;
    _oleaccDllService = oleaccDllService;
    _shCoreDll = shCoreDll;
    _settingsService = settingsService;
  }


  private nint? _windowHandle;
  public nint WindowHandle => _windowHandle ??= _pasteWindow.Handle;


  public Task<PasteWindowResult?> ShowWindowAsync(nint targetWindowToPasteHandle)
  {
    Point windowPosition = default;
    switch (_settingsService.PasteWindowSnappingType)
    {
      case PasteWindowSnappingType.Caret:
        var guid = typeof(IAccessible).GUID;
        _oleaccDllService.AccessibleObjectFromWindow(targetWindowToPasteHandle, OBJECT_IDENTIFIER.OBJID_CARET, guid, out var accessibleObject);
        var accessible = accessibleObject as IAccessible;
        if (accessible is not null)
        {
          accessible.accLocation(out var left, out var top, out _, out _, 0);
          windowPosition.X = left;
          windowPosition.Y = top;
        }
        if (windowPosition.X == 0 && windowPosition.Y == 0)
        {
          goto default;
        }
        break;
      case PasteWindowSnappingType.DisplayCorner:
        var monitorHandle = GetMonitorHandle(windowPosition);
        var monitorInfo = GetMonitorInfo(monitorHandle);
        windowPosition = _settingsService.PasteWindowSnappingDisplayCorner switch
        {
          PasteWindowSnappingDisplayCorner.TopLeft => new(monitorInfo.rcMonitor.X, monitorInfo.rcMonitor.Y),
          PasteWindowSnappingDisplayCorner.TopRight => new(monitorInfo.rcMonitor.Width - 1, monitorInfo.rcMonitor.Y),
          PasteWindowSnappingDisplayCorner.BottomLeft => new(monitorInfo.rcMonitor.X, monitorInfo.rcMonitor.Height - 1),
          PasteWindowSnappingDisplayCorner.BottomRight => new(monitorInfo.rcMonitor.Width - 1, monitorInfo.rcMonitor.Height - 1),
          _ => new(),
        };
        break;
      default:
        _user32Dll.GetCursorPos(out windowPosition);
        break;
    }

    _user32Dll.SetWindowPos(
      WindowHandle, nint.Zero, windowPosition.X, windowPosition.Y, 0, 0,
      SET_WINDOW_POS_FLAGS.SWP_NOSIZE | SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE | SET_WINDOW_POS_FLAGS.SWP_NOZORDER
    );

    _pasteWindow.Show();
    KeepWindowOnScreen(WindowHandle, windowPosition, _pasteWindow.ActualWidth, _pasteWindow.ActualHeight);
    _pasteWindow.Activate();
    return _pasteWindow.WaitForInputResultAsync();
  }


  public void HideWindow()
  {
    _pasteWindow.Hide();
  }


  private void KeepWindowOnScreen(nint windowHandle,
                                  Point windowPosition,
                                  double windowWidth,
                                  double windowHeight)
  {
    var monitorHandle = GetMonitorHandle(windowPosition);
    var monitorInfo = GetMonitorInfo(monitorHandle);
    _shCoreDll.GetDpiForMonitor(monitorHandle, MONITOR_DPI_TYPE.MDT_EFFECTIVE_DPI, out var dpiX, out var dpiY);

    var windowPixelWidth = windowWidth * dpiX / 96;
    var windowPixelHeight = windowHeight * dpiY / 96;

    var winLeft = windowPosition.X;
    var winTop = windowPosition.Y;
    if (winLeft + windowPixelWidth > monitorInfo.rcWork.right)
    {
      winLeft = monitorInfo.rcWork.right - (int) windowPixelWidth;
    }
    if (winTop + windowPixelHeight > monitorInfo.rcWork.bottom)
    {
      winTop = monitorInfo.rcWork.bottom - (int) windowPixelHeight;
    }

    if (winLeft != windowPosition.X || winTop != windowPosition.Y)
    {
      SetWindowPosition(windowHandle, winLeft, winTop);
    }
  }


  private nint GetMonitorHandle(Point windowPosition)
  {
    return _user32Dll.MonitorFromPoint(windowPosition, MONITOR_FROM_FLAGS.MONITOR_DEFAULTTONEAREST);
  }


  private MONITORINFO GetMonitorInfo(nint monitorHandle)
  {
    _user32Dll.GetMonitorInfo(monitorHandle, out var monitorInfo);
    return monitorInfo;
  }


  private void SetWindowPosition(nint windowHandle, int left, int top)
  {
    _user32Dll.SetWindowPos(windowHandle, nint.Zero, left, top, 0, 0,
      SET_WINDOW_POS_FLAGS.SWP_NOSIZE | SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE | SET_WINDOW_POS_FLAGS.SWP_NOZORDER
    );
  }
}
