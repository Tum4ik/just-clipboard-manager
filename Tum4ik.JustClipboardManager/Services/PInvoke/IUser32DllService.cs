using System.Runtime.InteropServices;
using Tum4ik.JustClipboardManager.Services.PInvoke.ParameterModels;

namespace Tum4ik.JustClipboardManager.Services.PInvoke;
internal interface IUser32DllService
{
  bool GetCursorPos(ref Win32Point pt);

  /// <summary>
  /// Changes the size, position, and Z order of a child, pop-up, or top-level window. These windows are ordered
  /// according to their appearance on the screen. The topmost window receives the highest rank and is the first
  /// window in the Z order.
  /// </summary>
  /// <param name="hWnd">A handle to the window.</param>
  /// <param name="hWndInsertAfter">
  /// A handle to the window to precede the positioned window in the Z order. This parameter must be a window handle
  /// or one of the following values.
  /// </param>
  /// <param name="x">The new position of the left side of the window, in client coordinates.</param>
  /// <param name="y">The new position of the top of the window, in client coordinates.</param>
  /// <param name="cx">The new width of the window, in pixels.</param>
  /// <param name="cy">The new height of the window, in pixels.</param>
  /// <param name="uFlags">The window sizing and positioning flags.</param>
  /// <returns>
  /// If the function succeeds, the return value is true.
  /// If the function fails, the return value is false.
  /// </returns>
  bool SetWindowPos(nint hWnd, nint hWndInsertAfter, int x, int y, int cx, int cy, SizingAndPositioning uFlags);

  /// <summary>
  /// Sets the specified window's show state.
  /// </summary>
  /// <param name="hWnd">A handle to the window.</param>
  /// <param name="nCmdShow">Controls how the window is to be shown.</param>
  /// <returns>
  /// If the window was previously visible, the return value is nonzero.
  /// If the window was previously hidden, the return value is zero.
  /// </returns>
  bool ShowWindow(nint hWnd, ShowWindowCommand nCmdShow);

  nint MonitorFromPoint(Win32Point pt, MonitorOptions dwFlags);

  /// <summary>
  /// Retrieves a handle to the display monitor that has the largest area of intersection with the bounding rectangle
  /// of a specified window.
  /// </summary>
  /// <param name="hwnd">A handle to the window of interest.</param>
  /// <param name="dwFlags">
  /// Determines the function's return value if the window does not intersect any display monitor.
  /// </param>
  /// <returns>
  /// If the window intersects one or more display monitor rectangles, the return value is an HMONITOR handle to
  /// the display monitor that has the largest area of intersection with the window.
  /// If the window does not intersect a display monitor, the return value depends on the value of dwFlags.
  /// </returns>
  /// <remarks>
  /// If the window is currently minimized, MonitorFromWindow uses the rectangle of the window before it was minimized.
  /// </remarks>
  nint MonitorFromWindow(nint hwnd, MonitorOptions dwFlags);

  /// <summary>
  /// Retrieves information about a display monitor.
  /// </summary>
  /// <param name="hMonitor">A handle to the display monitor of interest.</param>
  /// <param name="monitorInfo">Information about the specified display monitor.</param>
  /// <returns>
  /// If the function succeeds, the return value is true.
  /// If the function fails, the return value is false.
  /// </returns>
  bool GetMonitorInfo(nint hMonitor, out MonitorInfo monitorInfo);
}


internal class User32DllService : IUser32DllService
{
  public bool GetCursorPos(ref Win32Point pt)
  {
    return _GetCursorPos(ref pt);
  }


  public bool SetWindowPos(nint hWnd, nint hWndInsertAfter, int x, int y, int cx, int cy,
                            SizingAndPositioning uFlags)
  {
    return _SetWindowPos(hWnd, hWndInsertAfter, x, y, cx, cy, uFlags);
  }


  public bool ShowWindow(nint hWnd, ShowWindowCommand nCmdShow)
  {
    return _ShowWindow(hWnd, nCmdShow);
  }


  public nint MonitorFromPoint(Win32Point pt, MonitorOptions dwFlags)
  {
    return _MonitorFromPoint(pt, dwFlags);
  }


  public nint MonitorFromWindow(nint hwnd, MonitorOptions dwFlags)
  {
    return _MonitorFromWindow(hwnd, dwFlags);
  }


  public bool GetMonitorInfo(nint hMonitor, out MonitorInfo monitorInfo)
  {
    monitorInfo = new MonitorInfo();
    monitorInfo.Size = Marshal.SizeOf(monitorInfo);
    return _GetMonitorInfo(hMonitor, ref monitorInfo);
  }


  [DllImport("user32.dll", EntryPoint = "GetCursorPos")]
  [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
  [return: MarshalAs(UnmanagedType.Bool)]
  private static extern bool _GetCursorPos(ref Win32Point pt);


  [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
  [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
  [return: MarshalAs(UnmanagedType.Bool)]
  private static extern bool _SetWindowPos(
    nint hWnd, nint hWndInsertAfter, int x, int y, int cx, int cy, SizingAndPositioning uFlags
  );


  [DllImport("user32.dll", EntryPoint = "ShowWindow")]
  [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
  [return: MarshalAs(UnmanagedType.Bool)]
  private static extern bool _ShowWindow(nint hWnd, ShowWindowCommand nCmdShow);


  [DllImport("user32.dll", EntryPoint = "MonitorFromPoint")]
  [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
  private static extern nint _MonitorFromPoint(Win32Point pt, MonitorOptions dwFlags);


  [DllImport("user32.dll", EntryPoint = "MonitorFromWindow")]
  [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
  private static extern nint _MonitorFromWindow(nint hwnd, MonitorOptions dwFlags);


  [DllImport("user32.dll", EntryPoint = "GetMonitorInfo")]
  [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
  [return: MarshalAs(UnmanagedType.Bool)]
  private static extern bool _GetMonitorInfo(nint hMonitor, ref MonitorInfo lpmi);
}
