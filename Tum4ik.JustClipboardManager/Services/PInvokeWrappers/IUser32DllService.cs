using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Input;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Gdi;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using Windows.Win32.UI.WindowsAndMessaging;

namespace Tum4ik.JustClipboardManager.Services.PInvokeWrappers;
internal interface IUser32DllService
{
  /// <inheritdoc cref="PInvoke.GetCursorPos(out Point)"/>
  bool GetCursorPos(out Point lpPoint) => PInvoke.GetCursorPos(out lpPoint);

  /// <inheritdoc cref="PInvoke.SetWindowPos(HWND, HWND, int, int, int, int, SET_WINDOW_POS_FLAGS)"/>
  bool SetWindowPos(nint hWnd, nint hWndInsertAfter, int X, int Y, int cx, int cy, SET_WINDOW_POS_FLAGS uFlags)
    => PInvoke.SetWindowPos((HWND) hWnd, (HWND) hWndInsertAfter, X, Y, cx, cy, uFlags);

  /// <inheritdoc cref="PInvoke.ShowWindow(HWND, SHOW_WINDOW_CMD)"/>
  bool ShowWindow(nint hWnd, SHOW_WINDOW_CMD nCmdShow) => PInvoke.ShowWindow((HWND) hWnd, nCmdShow);

  /// <inheritdoc cref="PInvoke.GetForegroundWindow"/>
  nint GetForegroundWindow() => PInvoke.GetForegroundWindow();

  /// <inheritdoc cref="PInvoke.MonitorFromPoint(Point, MONITOR_FROM_FLAGS)"/>
  nint MonitorFromPoint(Point pt, MONITOR_FROM_FLAGS dwFlags) => PInvoke.MonitorFromPoint(pt, dwFlags);

  /// <inheritdoc cref="PInvoke.MonitorFromWindow(HWND, MONITOR_FROM_FLAGS)"/>
  nint MonitorFromWindow(nint hwnd, MONITOR_FROM_FLAGS dwFlags) => PInvoke.MonitorFromWindow((HWND) hwnd, dwFlags);

  /// <inheritdoc cref="PInvoke.GetMonitorInfo(HMONITOR, ref MONITORINFO)"/>
  bool GetMonitorInfo(nint hMonitor, out MONITORINFO monitorInfo)
  {
    monitorInfo = new MONITORINFO();
    monitorInfo.cbSize = (uint) Marshal.SizeOf(monitorInfo);
    return PInvoke.GetMonitorInfo((HMONITOR) hMonitor, ref monitorInfo);
  }

  /// <inheritdoc cref="PInvoke.RegisterHotKey(HWND, int, HOT_KEY_MODIFIERS, uint)"/>
  bool RegisterHotKey(nint hWnd, int id, ModifierKeys modifiers, Key key)
  {
    var vk = (uint) KeyInterop.VirtualKeyFromKey(key);
    return PInvoke.RegisterHotKey((HWND) hWnd, id, (HOT_KEY_MODIFIERS) modifiers, vk);
  }

  /// <inheritdoc cref="PInvoke.UnregisterHotKey(HWND, int)"/>
  bool UnregisterHotKey(nint hWnd, int id) => PInvoke.UnregisterHotKey((HWND) hWnd, id);

  /// <inheritdoc cref="PInvoke.AddClipboardFormatListener(HWND)"/>
  bool AddClipboardFormatListener(nint hwnd) => PInvoke.AddClipboardFormatListener((HWND) hwnd);

  /// <inheritdoc cref="PInvoke.RemoveClipboardFormatListener(HWND)"/>
  bool RemoveClipboardFormatListener(nint hwnd) => PInvoke.RemoveClipboardFormatListener((HWND) hwnd);

  /// <inheritdoc cref="PInvoke.SendInput(Span{INPUT}, int)"/>
  uint SendInput(Span<INPUT> pInputs, int cbSize) => PInvoke.SendInput(pInputs, cbSize);

  /// <inheritdoc cref="PInvoke.SetForegroundWindow(HWND)"/>
  bool SetForegroundWindow(nint hWnd) => PInvoke.SetForegroundWindow((HWND) hWnd);

  /// <inheritdoc cref="PInvoke.SetFocus(HWND)"/>
  nint SetFocus(nint hWnd) => PInvoke.SetFocus((HWND) hWnd);
}


internal class User32DllService : IUser32DllService { }
