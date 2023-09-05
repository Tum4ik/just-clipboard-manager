using System.Runtime.InteropServices;
using System.Windows.Input;
using Tum4ik.JustClipboardManager.Services.PInvoke.ParameterModels;

namespace Tum4ik.JustClipboardManager.Services.PInvoke;
internal interface IUser32DllService
{
  /// <summary>
  /// Retrieves the position of the mouse cursor, in screen coordinates.
  /// </summary>
  /// <param name="lpPoint">
  /// A pointer to a <see cref="POINT"/> structure that receives the screen coordinates of the cursor.
  /// </param>
  /// <returns>Returns true if successful or false otherwise.</returns>
  bool GetCursorPos(out POINT lpPoint) => _GetCursorPos(out lpPoint);

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
  /// <param name="X">The new position of the left side of the window, in client coordinates.</param>
  /// <param name="Y">The new position of the top of the window, in client coordinates.</param>
  /// <param name="cx">The new width of the window, in pixels.</param>
  /// <param name="cy">The new height of the window, in pixels.</param>
  /// <param name="uFlags">The window sizing and positioning flags.</param>
  /// <returns>
  /// If the function succeeds, the return value is true.
  /// If the function fails, the return value is false.
  /// </returns>
  bool SetWindowPos(nint hWnd, nint hWndInsertAfter, int X, int Y, int cx, int cy, SizingAndPositioning uFlags)
    => _SetWindowPos(hWnd, hWndInsertAfter, X, Y, cx, cy, uFlags);

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

  /// <summary>
  /// Retrieves a handle to the display monitor that contains a specified point.
  /// </summary>
  /// <param name="pt">Specifies the point of interest in virtual-screen coordinates.</param>
  /// <param name="dwFlags">
  /// Determines the function's return value if the point is not contained within any display monitor.
  /// This parameter can be one of the values.
  /// </param>
  /// <returns>
  /// If the point is contained by a display monitor, the return value is a handle to that display monitor.
  /// If the point is not contained by a display monitor,
  /// the return value depends on the value of <see cref="dwFlags"/>.
  /// </returns>
  nint MonitorFromPoint(POINT pt, MonitorOptions dwFlags) => _MonitorFromPoint(pt, dwFlags);

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
    monitorInfo.Size = (uint) Marshal.SizeOf(monitorInfo);
    return _GetMonitorInfo(hMonitor, ref monitorInfo);
  }

  /// <summary>
  /// Defines a system-wide hot key.
  /// </summary>
  /// <param name="hWnd">
  /// A handle to the window that will receive WM_HOTKEY messages generated by the hot key.
  /// If this parameter is NULL, WM_HOTKEY messages are posted to the message queue of the calling thread
  /// and must be processed in the message loop.
  /// </param>
  /// <param name="id">
  /// The identifier of the hot key. If the hWnd parameter is NULL,
  /// then the hot key is associated with the current thread rather than with a particular window.
  /// </param>
  /// <param name="modifiers">The keys that must be pressed in combination with the <see cref="key"/>.</param>
  /// <param name="key">The key code of the hot key.</param>
  /// <returns>
  /// If the function succeeds, the return value is true.
  /// If the function fails, the return value is false.
  /// </returns>
  bool RegisterHotKey(nint hWnd, int id, ModifierKeys modifiers, Key key)
  {
    var vk = (uint) KeyInterop.VirtualKeyFromKey(key);
    return _RegisterHotKey(hWnd, id, (uint) modifiers, vk);
  }

  /// <summary>
  /// Frees a hot key previously registered by the calling thread.
  /// </summary>
  /// <param name="hWnd">
  /// A handle to the window associated with the hot key to be freed.
  /// This parameter should be NULL if the hot key is not associated with a window.
  /// </param>
  /// <param name="id">The identifier of the hot key to be freed.</param>
  /// <returns>
  /// If the function succeeds, the return value is true.
  /// If the function fails, the return value is false.
  /// </returns>
  bool UnregisterHotKey(nint hWnd, int id) => _UnregisterHotKey(hWnd, id);

  /// <summary>
  /// Places the given window in the system-maintained clipboard format listener list.
  /// </summary>
  /// <param name="hwnd">A handle to the window to be placed in the clipboard format listener list.</param>
  /// <returns>Returns TRUE if successful, FALSE otherwise</returns>
  bool AddClipboardFormatListener(nint hwnd) => _AddClipboardFormatListener(hwnd);

  /// <summary>
  /// Removes the given window from the system-maintained clipboard format listener list.
  /// </summary>
  /// <param name="hwnd">A handle to the window to remove from the clipboard format listener list.</param>
  /// <returns>Returns TRUE if successful, FALSE otherwise.</returns>
  bool RemoveClipboardFormatListener(nint hwnd) => _RemoveClipboardFormatListener(hwnd);

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

  /// <summary>
  /// Brings the thread that created the specified window into the foreground and activates the window.
  /// Keyboard input is directed to the window, and various visual cues are changed for the user.
  /// The system assigns a slightly higher priority to the thread that created the foreground window
  /// than it does to other threads.
  /// </summary>
  /// <param name="hWnd">A handle to the window that should be activated and brought to the foreground.</param>
  /// <returns>
  /// If the window was brought to the foreground, the return value is true.
  /// If the window was not brought to the foreground, the return value is false.
  /// </returns>
  bool SetForegroundWindow(nint hWnd) => _SetForegroundWindow(hWnd);

  /// <summary>
  /// Sets the keyboard focus to the specified window.
  /// The window must be attached to the calling thread's message queue.
  /// </summary>
  /// <param name="hWnd">
  /// A handle to the window that will receive the keyboard input. If this parameter is NULL, keystrokes are ignored.
  /// </param>
  /// <returns>
  /// If the function succeeds, the return value is the handle to the window that previously had the keyboard focus.
  /// If the hWnd parameter is invalid or the window is not attached to the calling thread's message queue,
  /// the return value is NULL.
  /// </returns>
  nint SetFocus(nint hWnd) => _SetFocus(hWnd);


  [DllImport("user32.dll", EntryPoint = "GetCursorPos")]
  [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
  [return: MarshalAs(UnmanagedType.Bool)]
  private static extern bool _GetCursorPos(out POINT lpPoint);


  [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
  [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
  [return: MarshalAs(UnmanagedType.Bool)]
  private static extern bool _SetWindowPos(
    nint hWnd, nint hWndInsertAfter, int X, int Y, int cx, int cy, SizingAndPositioning uFlags
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
  private static extern nint _MonitorFromPoint(POINT pt, MonitorOptions dwFlags);


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
  private static extern bool _RegisterHotKey(nint hWnd, int id, uint fsModifiers, uint vk);


  [DllImport("user32.dll", EntryPoint = "UnregisterHotKey")]
  [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
  [return: MarshalAs(UnmanagedType.Bool)]
  private static extern bool _UnregisterHotKey(nint hWnd, int id);


  [DllImport("user32.dll", EntryPoint = "AddClipboardFormatListener", SetLastError = true)]
  [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
  [return: MarshalAs(UnmanagedType.Bool)]
  private static extern bool _AddClipboardFormatListener(nint hwnd);


  [DllImport("user32.dll", EntryPoint = "RemoveClipboardFormatListener", SetLastError = true)]
  [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
  [return: MarshalAs(UnmanagedType.Bool)]
  private static extern bool _RemoveClipboardFormatListener(nint hwnd);


  [DllImport("user32.dll", EntryPoint = "SendInput")]
  [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
  private static extern uint _SendInput(uint cInputs, INPUT[] pInputs, int cbSize);


  [DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
  [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
  [return: MarshalAs(UnmanagedType.Bool)]
  private static extern bool _SetForegroundWindow(nint hWnd);


  [DllImport("user32.dll", EntryPoint = "SetFocus")]
  [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
  private static extern nint _SetFocus(nint hWnd);
}


internal class User32DllService : IUser32DllService { }
