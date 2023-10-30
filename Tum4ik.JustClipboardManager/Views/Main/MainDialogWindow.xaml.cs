using System.Windows;
using System.Windows.Interop;
using Prism.Services.Dialogs;
using Tum4ik.JustClipboardManager.Services.Dialogs;
using Tum4ik.JustClipboardManager.Services.PInvokeWrappers;
using Windows.Win32.UI.HiDpi;
using Windows.Win32.Graphics.Gdi;
using static Windows.Win32.PInvoke;

namespace Tum4ik.JustClipboardManager.Views.Main;

/// <summary>
/// Interaction logic for MainDialogWindow.xaml
/// </summary>
internal partial class MainDialogWindow : IDialogWindowExtended
{
  private readonly IUser32DllService _user32Dll;
  private readonly ISHCoreDllService _shCoreDll;

  public MainDialogWindow(IUser32DllService user32Dll,
                          ISHCoreDllService shCoreDll)
  {
    _user32Dll = user32Dll;
    _shCoreDll = shCoreDll;

    InitializeComponent();

    HwndSource.FromHwnd(Handle).AddHook(HwndHook);
    _initialMargin = Margin;
  }


  private readonly Thickness _initialMargin;


  public IDialogResult? Result { get; set; }


  private nint? _handle;
  public nint Handle => _handle ??= new WindowInteropHelper(this).EnsureHandle();


  private nint HwndHook(nint hWnd, int msg, nint wParam, nint lParam, ref bool handled)
  {
    switch ((uint) msg)
    {
      case WM_NCLBUTTONDBLCLK:
        if (WindowState == WindowState.Normal)
        {
          SystemCommands.MaximizeWindow(this);
          handled = true;
        }
        break;
      case WM_SYSCOMMAND:
        switch ((uint) wParam)
        {
          case SC_MAXIMIZE:
          case 0xF032: // SC_MAXIMIZE_DBLCLICK
            BeforeMaximize();
            break;
          case SC_RESTORE:
          case 0xF122: // SC_RESTORE_DBLCLICK
            BeforeRestore();
            break;
        }
        break;
    }
    return nint.Zero;
  }


  private void BeforeMaximize()
  {
    var monitorHandle = _user32Dll.MonitorFromWindow(Handle, MONITOR_FROM_FLAGS.MONITOR_DEFAULTTONEAREST);
    if (_user32Dll.GetMonitorInfo(monitorHandle, out var monitorInfo)
        && _shCoreDll.GetDpiForMonitor(monitorHandle, MONITOR_DPI_TYPE.MDT_EFFECTIVE_DPI, out var dpiX, out var dpiY))
    {
      MaxWidth = (monitorInfo.rcWork.right - monitorInfo.rcWork.left) / (dpiX / 96d);
      MaxHeight = (monitorInfo.rcWork.bottom - monitorInfo.rcWork.top) / (dpiY / 96d);
      Margin = new(0, 0, 0, 0);
    }
  }


  private void BeforeRestore()
  {
    Margin = _initialMargin;
  }


  private void MinimizeButton_Click(object sender, RoutedEventArgs e)
  {
    SystemCommands.MinimizeWindow(this);
  }


  private void MaximizeRestoreButton_Click(object sender, RoutedEventArgs e)
  {
    if (WindowState == WindowState.Normal)
    {
      SystemCommands.MaximizeWindow(this);
    }
    else if (WindowState == WindowState.Maximized)
    {
      SystemCommands.RestoreWindow(this);
    }
  }


  private void CloseButton_Click(object sender, RoutedEventArgs e)
  {
    Result = new DialogResult();
    Close();
  }
}
