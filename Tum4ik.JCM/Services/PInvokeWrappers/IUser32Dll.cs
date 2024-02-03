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
}


internal sealed class User32Dll : IUser32Dll { }
