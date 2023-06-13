using System.Runtime.InteropServices;
using Tum4ik.JustClipboardManager.Services.PInvoke.ParameterModels;

namespace Tum4ik.JustClipboardManager.Services.PInvoke;
internal interface IUser32DllService
{
  bool GetCursorPos(ref Win32Point pt) => _GetCursorPos(ref pt);

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
  bool SetWindowPos(nint hWnd, nint hWndInsertAfter, int x, int y, int cx, int cy, SizingAndPositioning uFlags)
    => _SetWindowPos(hWnd, hWndInsertAfter, x, y, cx, cy, uFlags);

  /// <summary>
  /// Sets the specified window's show state.
  /// </summary>
  /// <param name="hWnd">A handle to the window.</param>
  /// <param name="nCmdShow">Controls how the window is to be shown.</param>
  /// <returns>
  /// If the window was previously visible, the return value is nonzero.
  /// If the window was previously hidden, the return value is zero.
  /// </returns>
  bool ShowWindow(nint hWnd, ShowWindowCommand nCmdShow) => _ShowWindow(hWnd, nCmdShow);

  /// <summary>
  /// Retrieves a handle to the foreground window (the window with which the user is currently working). The system
  /// assigns a slightly higher priority to the thread that creates the foreground window than it does to other threads.
  /// </summary>
  /// <returns>
  /// C++ ( Type: Type: HWND )<br /> The return value is a handle to the foreground window. The foreground window
  /// can be NULL in certain circumstances, such as when a window is losing activation.
  /// </returns>
  nint GetForegroundWindow() => _GetForegroundWindow();

  nint MonitorFromPoint(Win32Point pt, MonitorOptions dwFlags) => _MonitorFromPoint(pt, dwFlags);

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
  nint MonitorFromWindow(nint hwnd, MonitorOptions dwFlags) => _MonitorFromWindow(hwnd, dwFlags);

  /// <summary>
  /// Retrieves information about a display monitor.
  /// </summary>
  /// <param name="hMonitor">A handle to the display monitor of interest.</param>
  /// <param name="monitorInfo">Information about the specified display monitor.</param>
  /// <returns>
  /// If the function succeeds, the return value is true.
  /// If the function fails, the return value is false.
  /// </returns>
  bool GetMonitorInfo(nint hMonitor, out MonitorInfo monitorInfo)
  {
    monitorInfo = new MonitorInfo();
    monitorInfo.Size = Marshal.SizeOf(monitorInfo);
    return _GetMonitorInfo(hMonitor, ref monitorInfo);
  }

  bool RegisterHotKey(nint hWnd, int id, int modifiers, int vKey) => _RegisterHotKey(hWnd, id, modifiers, vKey);

  bool UnregisterHotKey(nint hWnd, int id) => _UnregisterHotKey(hWnd, id);

  nint SetClipboardViewer(nint hWndNewViewer) => _SetClipboardViewer(hWndNewViewer);

  bool ChangeClipboardChain(nint hWndRemove, nint hWndNewNext) => _ChangeClipboardChain(hWndRemove, hWndNewNext);

  int SendMessage(nint hWnd, int msg, nint wParam, nint lParam) => _SendMessage(hWnd, msg, wParam, lParam);

  /// <summary>
  /// Synthesizes keystrokes, mouse motions, and button clicks.
  /// </summary>
  /// <param name="cInputs">The number of structures in the pInputs array.</param>
  /// <param name="pInputs">
  /// An array of <see cref="INPUT"/> structures.
  /// Each structure represents an event to be inserted into the keyboard or mouse input stream.
  /// </param>
  /// <param name="cbSize">
  /// The size, in bytes, of an <see cref="INPUT"/> structure.
  /// If cbSize is not the size of an <see cref="INPUT"/> structure, the function fails.
  /// </param>
  /// <returns>
  /// The function returns the number of events that it successfully inserted into the keyboard or mouse input stream.
  /// If the function returns zero, the input was already blocked by another thread.
  /// </returns>
  uint SendInput(uint cInputs, INPUT[] pInputs, int cbSize) => _SendInput(cInputs, pInputs, cbSize);

  bool SetForegroundWindow(nint hWnd) => _SetForegroundWindow(hWnd);

  int SetFocus(nint hWnd) => _SetFocus(hWnd);


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


  [DllImport("user32.dll", EntryPoint = "GetForegroundWindow")]
  [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
  private static extern nint _GetForegroundWindow();


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


  [DllImport("user32.dll", EntryPoint = "RegisterHotKey")]
  [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
  [return: MarshalAs(UnmanagedType.Bool)]
  private static extern bool _RegisterHotKey(nint hWnd, int id, int modifiers, int vKey);


  [DllImport("user32.dll", EntryPoint = "UnregisterHotKey")]
  [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
  [return: MarshalAs(UnmanagedType.Bool)]
  private static extern bool _UnregisterHotKey(nint hWnd, int id);


  [DllImport("user32.dll", EntryPoint = "SetClipboardViewer")]
  [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
  private static extern nint _SetClipboardViewer(nint hWndNewViewer);


  [DllImport("user32.dll", EntryPoint = "ChangeClipboardChain")]
  [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
  [return: MarshalAs(UnmanagedType.Bool)]
  private static extern bool _ChangeClipboardChain(nint hWndRemove, nint hWndNewNext);


  [DllImport("user32.dll", EntryPoint = "SendMessage")]
  [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
  private static extern int _SendMessage(nint hWnd, int msg, nint wParam, nint lParam);


  [DllImport("user32.dll", EntryPoint = "SendInput")]
  [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
  private static extern uint _SendInput(uint cInputs, INPUT[] pInputs, int cbSize);


  [DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
  [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
  private static extern bool _SetForegroundWindow(nint hWnd);


  [DllImport("user32.dll", EntryPoint = "SetFocus")]
  [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
  private static extern int _SetFocus(nint hWnd);
}


internal class User32DllService : IUser32DllService { }
