using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using Accessibility;
using Tum4ik.JustClipboardManager.Views;

namespace Tum4ik.JustClipboardManager.Services;
internal class PasteWindowService : IPasteWindowService
{
  private readonly PasteWindow _pasteWindow;

  public PasteWindowService(PasteWindow pasteWindow)
  {
    _pasteWindow = pasteWindow;
    WindowHandle = new WindowInteropHelper(_pasteWindow).EnsureHandle();
  }


  public IntPtr WindowHandle { get; }


  private Win32Point _windowPosition;
  private const bool IsSnapToMousePosition = true;

  public void ShowWindow(IntPtr targetWindowToPasteHandle)
  {
    if (IsSnapToMousePosition)
    {
      GetCursorPos(ref _windowPosition);
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
        _windowPosition.X = left;
        _windowPosition.Y = top;
      }
    }

    SetWindowPos(
      WindowHandle, IntPtr.Zero, _windowPosition.X, _windowPosition.Y, 0, 0, SWP_NOSIZE | SWP_NOACTIVATE | SWP_NOZORDER
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
    var monitorHandle = MonitorFromPoint(_windowPosition, MonitorOptions.MONITOR_DEFAULTTONEAREST);
    GetDpiForMonitor(monitorHandle, MonitorDpiType.MDT_EFFECTIVE_DPI, out var dpiX, out var dpiY);

    var windowPixelWidth = e.NewSize.Width * dpiX / 96;
    var windowPixelHeight = e.NewSize.Height * dpiY / 96;
    
    var monitorInfo = new MonitorInfo();
    monitorInfo.Size = Marshal.SizeOf(monitorInfo);
    GetMonitorInfo(monitorHandle, ref monitorInfo);

    var winLeft = _windowPosition.X;
    var winTop = _windowPosition.Y;
    if (winLeft + windowPixelWidth > monitorInfo.WorkArea.Right)
    {
      winLeft = monitorInfo.WorkArea.Right - (int) windowPixelWidth;
    }
    if (winTop + windowPixelHeight > monitorInfo.WorkArea.Bottom)
    {
      winTop = Math.Min(monitorInfo.WorkArea.Bottom - (int) windowPixelHeight, winTop - (int) windowPixelHeight);
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


  private void SetWindowPosition(int left, int top)
  {
    SetWindowPos(WindowHandle, IntPtr.Zero, left, top, 0, 0, SWP_NOSIZE | SWP_NOACTIVATE | SWP_NOZORDER);
  }


  [DllImport("user32.dll")]
  [return: MarshalAs(UnmanagedType.Bool)]
  private static extern bool GetCursorPos(ref Win32Point pt);

  [StructLayout(LayoutKind.Sequential)]
  internal struct Win32Point
  {
    public int X { get; set; }
    public int Y { get; set; }
  };


  [DllImport("user32.dll")]
  [return: MarshalAs(UnmanagedType.Bool)]
  private static extern bool SetWindowPos(
    IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, int uFlags
  );

  private const int SWP_NOSIZE = 0x0001;
  private const int SWP_NOACTIVATE = 0x0010;
  private const int SWP_NOZORDER = 0x0004;


  /// <summary>
  /// Retrieves the address of the specified interface for the object associated with the specified window.
  /// </summary>
  [DllImport("oleacc.dll")]
  private static extern int AccessibleObjectFromWindow(
    IntPtr hwnd, OBJID id, ref Guid iid, [In, Out, MarshalAs(UnmanagedType.IUnknown)] ref object ppvObject
  );


  [DllImport("user32.dll", SetLastError = true)]
  private static extern IntPtr MonitorFromPoint(Win32Point pt, MonitorOptions dwFlags);


  [DllImport("Shcore.dll")]
  private static extern bool GetDpiForMonitor(IntPtr hwnd, MonitorDpiType dpiType, out int dpiX, out int dpiY);


  [DllImport("user32.dll")]
  private static extern bool GetMonitorInfo(IntPtr hMonitor, [In, Out] ref MonitorInfo lpmi);
}


internal enum OBJID : uint
{
  WINDOW = 0x00000000,
  SYSMENU = 0xFFFFFFFF,
  TITLEBAR = 0xFFFFFFFE,
  MENU = 0xFFFFFFFD,
  CLIENT = 0xFFFFFFFC,
  VSCROLL = 0xFFFFFFFB,
  HSCROLL = 0xFFFFFFFA,
  SIZEGRIP = 0xFFFFFFF9,
  CARET = 0xFFFFFFF8,
  CURSOR = 0xFFFFFFF7,
  ALERT = 0xFFFFFFF6,
  SOUND = 0xFFFFFFF5,
}


internal enum MonitorOptions : uint
{
  MONITOR_DEFAULTTONULL = 0x00000000,
  MONITOR_DEFAULTTOPRIMARY = 0x00000001,
  MONITOR_DEFAULTTONEAREST = 0x00000002
}


internal enum MonitorDpiType
{
  /// <summary>
  /// The effective DPI.
  /// This value should be used when determining the correct scale factor for scaling UI elements.
  /// This incorporates the scale factor set by the user for this specific display.
  /// </summary>
  MDT_EFFECTIVE_DPI = 0,
  /// <summary>
  /// The angular DPI.
  /// This DPI ensures rendering at a compliant angular resolution on the screen.
  /// This does not include the scale factor set by the user for this specific display.
  /// </summary>
  MDT_ANGULAR_DPI = 1,
  /// <summary>
  /// The raw DPI.
  /// This value is the linear DPI of the screen as measured on the screen itself.
  /// Use this value when you want to read the pixel density and not the recommended scaling setting.
  /// This does not include the scale factor set by the user for this specific display and is not guaranteed to be a 
  /// supported DPI value.
  /// </summary>
  MDT_RAW_DPI = 2
}


[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
internal struct MonitorInfo
{
  /// <summary>
  /// The size, in bytes, of the structure. Set this member to sizeof(MONITORINFOEX) (72) before calling the GetMonitorInfo function.
  /// Doing so lets the function determine the type of structure you are passing to it.
  /// </summary>
  public int Size { get; set; }

  /// <summary>
  /// A RECT structure that specifies the display monitor rectangle, expressed in virtual-screen coordinates.
  /// Note that if the monitor is not the primary display monitor, some of the rectangle's coordinates may be negative values.
  /// </summary>
  public RectStruct Monitor { get; set; }

  /// <summary>
  /// A RECT structure that specifies the work area rectangle of the display monitor that can be used by applications,
  /// expressed in virtual-screen coordinates. Windows uses this rectangle to maximize an application on the monitor.
  /// The rest of the area in rcMonitor contains system windows such as the task bar and side bars.
  /// Note that if the monitor is not the primary display monitor, some of the rectangle's coordinates may be negative values.
  /// </summary>
  public RectStruct WorkArea { get; set; }

  /// <summary>
  /// The attributes of the display monitor.
  ///
  /// This member can be the following value:
  ///   1 : MONITORINFOF_PRIMARY
  /// </summary>
  public uint Flags { get; set; }
}


/// <summary>
/// The RECT structure defines the coordinates of the upper-left and lower-right corners of a rectangle.
/// </summary>
/// <remarks>
/// By convention, the right and bottom edges of the rectangle are normally considered exclusive.
/// In other words, the pixel whose coordinates are ( right, bottom ) lies immediately outside of the the rectangle.
/// For example, when RECT is passed to the FillRect function, the rectangle is filled up to, but not including,
/// the right column and bottom row of pixels. This structure is identical to the RECTL structure.
/// </remarks>
[StructLayout(LayoutKind.Sequential)]
public struct RectStruct
{
  /// <summary>
  /// The x-coordinate of the upper-left corner of the rectangle.
  /// </summary>
  public int Left { get; set; }

  /// <summary>
  /// The y-coordinate of the upper-left corner of the rectangle.
  /// </summary>
  public int Top { get; set; }

  /// <summary>
  /// The x-coordinate of the lower-right corner of the rectangle.
  /// </summary>
  public int Right { get; set; }

  /// <summary>
  /// The y-coordinate of the lower-right corner of the rectangle.
  /// </summary>
  public int Bottom { get; set; }
}
