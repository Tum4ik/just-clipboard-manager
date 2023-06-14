using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using Accessibility;
using Tum4ik.JustClipboardManager.Services.PInvoke;
using Tum4ik.JustClipboardManager.Services.PInvoke.ParameterModels;
using Tum4ik.JustClipboardManager.Views;

namespace Tum4ik.JustClipboardManager.Services;
internal class PasteWindowService : IPasteWindowService
{
  private readonly PasteWindow _pasteWindow;
  private readonly IUser32DllService _user32Dll;
  private readonly ISHCoreDllService _shCoreDll;

  public PasteWindowService(PasteWindow pasteWindow,
                            IUser32DllService user32Dll,
                            ISHCoreDllService shCoreDll)
  {
    _pasteWindow = pasteWindow;
    _user32Dll = user32Dll;
    _shCoreDll = shCoreDll;
    WindowHandle = new WindowInteropHelper(_pasteWindow).EnsureHandle();
  }


  public nint WindowHandle { get; }


  private POINT _windowPosition;
  private const bool IsSnapToMousePosition = true;

  public void ShowWindow(nint targetWindowToPasteHandle)
  {
    if (IsSnapToMousePosition)
    {
      _user32Dll.GetCursorPos(out _windowPosition);
    }
    else
    {
      // TODO: should be configured by application options someday :)
      // also good to have an option to snap paste window to one of the corner of the screen

      // snap to caret position
      var guid = typeof(IAccessible).GUID;
      object? accessibleObject = null;
      AccessibleObjectFromWindow(targetWindowToPasteHandle, OBJID.CARET, ref guid, ref accessibleObject);
      var accessible = accessibleObject as IAccessible;
      if (accessible is not null)
      {
        accessible.accLocation(out var left, out var top, out _, out _, 0);
        _windowPosition.x = left;
        _windowPosition.y = top;
      }
    }

    _user32Dll.SetWindowPos(
      WindowHandle, nint.Zero, _windowPosition.x, _windowPosition.y, 0, 0,
      SizingAndPositioning.SWP_NOSIZE | SizingAndPositioning.SWP_NOACTIVATE | SizingAndPositioning.SWP_NOZORDER
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

  
  private void PasteWindow_SizeChanged(object? sender, SizeChangedEventArgs e)
  {
    var monitorHandle = _user32Dll.MonitorFromPoint(_windowPosition, MonitorOptions.MONITOR_DEFAULTTONEAREST);
    _shCoreDll.GetDpiForMonitor(monitorHandle, MonitorDpiType.MDT_EFFECTIVE_DPI, out var dpiX, out var dpiY);

    var windowPixelWidth = e.NewSize.Width * dpiX / 96;
    var windowPixelHeight = e.NewSize.Height * dpiY / 96;

    _user32Dll.GetMonitorInfo(monitorHandle, out var monitorInfo);

    var winLeft = _windowPosition.x;
    var winTop = _windowPosition.y;
    if (winLeft + windowPixelWidth > monitorInfo.WorkArea.Right)
    {
      winLeft = monitorInfo.WorkArea.Right - (int) windowPixelWidth;
    }
    if (winTop + windowPixelHeight > monitorInfo.WorkArea.Bottom)
    {
      winTop = Math.Min(monitorInfo.WorkArea.Bottom - (int) windowPixelHeight, winTop - (int) windowPixelHeight);
    }

    if (winLeft != _windowPosition.x || winTop != _windowPosition.y)
    {
      SetWindowPosition(winLeft, winTop);
    }
  }


  private void PasteWindow_Deactivated(object? sender, EventArgs e)
  {
    _pasteWindow.SizeChanged -= PasteWindow_SizeChanged;
    _pasteWindow.Deactivated -= PasteWindow_Deactivated;
  }


  private void SetWindowPosition(int left, int top)
  {
    _user32Dll.SetWindowPos(WindowHandle, nint.Zero, left, top, 0, 0,
      SizingAndPositioning.SWP_NOSIZE | SizingAndPositioning.SWP_NOACTIVATE | SizingAndPositioning.SWP_NOZORDER
    );
  }




  /// <summary>
  /// Retrieves the address of the specified interface for the object associated with the specified window.
  /// </summary>
  [DllImport("oleacc.dll")]
  [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
  private static extern int AccessibleObjectFromWindow(
    nint hwnd, OBJID id, ref Guid iid, [In, Out, MarshalAs(UnmanagedType.IUnknown)] ref object ppvObject
  );
}
