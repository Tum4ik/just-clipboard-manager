using System.Drawing;
using System.Windows.Interop;
using Accessibility;
using Tum4ik.JustClipboardManager.Services.PInvokeWrappers;
using Tum4ik.JustClipboardManager.Views;
using Windows.Win32.UI.WindowsAndMessaging;
using Windows.Win32.UI.HiDpi;
using Windows.Win32.Graphics.Gdi;

namespace Tum4ik.JustClipboardManager.Services;
internal class PasteWindowService : IPasteWindowService
{
  private readonly PasteWindow _pasteWindow;
  private readonly IUser32DllService _user32Dll;
  private readonly ISHCoreDllService _shCoreDll;
  private readonly IOleaccDllService _oleaccDllService;
  private readonly ISettingsService _settingsService;

  public PasteWindowService(PasteWindow pasteWindow,
                            IUser32DllService user32Dll,
                            ISHCoreDllService shCoreDll,
                            IOleaccDllService oleaccDllService,
                            ISettingsService settingsService)
  {
    _pasteWindow = pasteWindow;
    _user32Dll = user32Dll;
    _shCoreDll = shCoreDll;
    _oleaccDllService = oleaccDllService;
    _settingsService = settingsService;
    WindowHandle = new WindowInteropHelper(_pasteWindow).EnsureHandle();
  }


  public nint WindowHandle { get; }


  private Point _windowPosition;

  public void ShowWindow(nint targetWindowToPasteHandle)
  {
    switch (_settingsService.PasteWindowSnappingType)
    {
      case PasteWindowSnappingType.Caret:
        var guid = typeof(IAccessible).GUID;
        _oleaccDllService.AccessibleObjectFromWindow(targetWindowToPasteHandle, OBJECT_IDENTIFIER.OBJID_CARET, guid, out var accessibleObject);
        var accessible = accessibleObject as IAccessible;
        if (accessible is not null)
        {
          accessible.accLocation(out var left, out var top, out _, out _, 0);
          _windowPosition.X = left;
          _windowPosition.Y = top;
        }
        if (_windowPosition.X == 0 && _windowPosition.Y == 0)
        {
          goto default;
        }
        break;
      case PasteWindowSnappingType.DisplayCorner:
        var monitorHandle = GetMonitorHandle();
        var monitorInfo = GetMonitorInfo(monitorHandle);
        _windowPosition = _settingsService.PasteWindowSnappingDisplayCorner switch
        {
          PasteWindowSnappingDisplayCorner.TopLeft => new(monitorInfo.rcMonitor.X, monitorInfo.rcMonitor.Y),
          PasteWindowSnappingDisplayCorner.TopRight => new(monitorInfo.rcMonitor.Width - 1, monitorInfo.rcMonitor.Y),
          PasteWindowSnappingDisplayCorner.BottomLeft => new(monitorInfo.rcMonitor.X, monitorInfo.rcMonitor.Height - 1),
          PasteWindowSnappingDisplayCorner.BottomRight => new(monitorInfo.rcMonitor.Width - 1, monitorInfo.rcMonitor.Height - 1),
          _ => throw new NotImplementedException(),
        };
        break;
      default:
      case PasteWindowSnappingType.Mouse:
        _user32Dll.GetCursorPos(out _windowPosition);
        break;
    }

    _user32Dll.SetWindowPos(
      WindowHandle, nint.Zero, _windowPosition.X, _windowPosition.Y, 0, 0,
      SET_WINDOW_POS_FLAGS.SWP_NOSIZE | SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE | SET_WINDOW_POS_FLAGS.SWP_NOZORDER
    );
    _pasteWindow.SizeChanged += PasteWindow_SizeChanged;
    _pasteWindow.Deactivated += PasteWindow_Deactivated;
    _pasteWindow.Show();
    _pasteWindow.Activate();
  }


  public void HideWindow()
  {
    _pasteWindow.Hide();
  }


  private void PasteWindow_SizeChanged(object? sender, System.Windows.SizeChangedEventArgs e)
  {
    var monitorHandle = GetMonitorHandle();
    var monitorInfo = GetMonitorInfo(monitorHandle);
    _shCoreDll.GetDpiForMonitor(monitorHandle, MONITOR_DPI_TYPE.MDT_EFFECTIVE_DPI, out var dpiX, out var dpiY);

    var windowPixelWidth = e.NewSize.Width * dpiX / 96;
    var windowPixelHeight = e.NewSize.Height * dpiY / 96;

    var winLeft = _windowPosition.X;
    var winTop = _windowPosition.Y;
    if (winLeft + windowPixelWidth > monitorInfo.rcWork.right)
    {
      winLeft = monitorInfo.rcWork.right - (int) windowPixelWidth;
    }
    if (winTop + windowPixelHeight > monitorInfo.rcWork.bottom)
    {
      winTop = Math.Min(monitorInfo.rcWork.bottom - (int) windowPixelHeight, winTop - (int) windowPixelHeight);
    }

    if (winLeft != _windowPosition.X || winTop != _windowPosition.Y)
    {
      SetWindowPosition(winLeft, winTop);
    }
  }


  private void PasteWindow_Deactivated(object? sender, EventArgs e)
  {
    _pasteWindow.SizeChanged -= PasteWindow_SizeChanged;
    _pasteWindow.Deactivated -= PasteWindow_Deactivated;
  }


  private nint GetMonitorHandle()
  {
    return _user32Dll.MonitorFromPoint(_windowPosition, MONITOR_FROM_FLAGS.MONITOR_DEFAULTTONEAREST);
  }


  private MONITORINFO GetMonitorInfo(nint monitorHandle)
  {
    _user32Dll.GetMonitorInfo(monitorHandle, out var monitorInfo);
    return monitorInfo;
  }


  private void SetWindowPosition(int left, int top)
  {
    _user32Dll.SetWindowPos(WindowHandle, nint.Zero, left, top, 0, 0,
      SET_WINDOW_POS_FLAGS.SWP_NOSIZE | SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE | SET_WINDOW_POS_FLAGS.SWP_NOZORDER
    );
  }
}
