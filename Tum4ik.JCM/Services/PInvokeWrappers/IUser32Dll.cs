using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

namespace Tum4ik.JustClipboardManager.Services.PInvokeWrappers;
internal interface IUser32Dll
{
  /// <inheritdoc cref="PInvoke.GetCursorPos(out System.Drawing.Point)"/>
  bool GetCursorPos(out System.Drawing.Point lpPoint) => PInvoke.GetCursorPos(out lpPoint);

  /// <inheritdoc cref="PInvoke.ShowWindow(HWND, SHOW_WINDOW_CMD)"/>
  bool ShowWindow(HWND hWnd, SHOW_WINDOW_CMD nCmdShow) => PInvoke.ShowWindow(hWnd, nCmdShow);

  /// <inheritdoc cref="PInvoke.SetForegroundWindow(HWND)"/>
  bool SetForegroundWindow(HWND hWnd) => PInvoke.SetForegroundWindow(hWnd);

  /// <inheritdoc cref="PInvoke.SendMessage(HWND, uint, WPARAM, LPARAM)"/>
  LRESULT SendMessage(HWND hWnd, uint Msg, WPARAM wParam, LPARAM lParam) =>
    PInvoke.SendMessage(hWnd, Msg, wParam, lParam);
}


internal sealed class User32Dll : IUser32Dll { }
