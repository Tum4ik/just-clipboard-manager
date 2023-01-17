using System.Windows;
using System.Windows.Interop;
using Prism.Services.Dialogs;
using Tum4ik.JustClipboardManager.Services.PInvoke;
using Tum4ik.JustClipboardManager.Services.PInvoke.ParameterModels;

namespace Tum4ik.JustClipboardManager.Views.Main;

/// <summary>
/// Interaction logic for MainDialogWindow.xaml
/// </summary>
internal partial class MainDialogWindow : IDialogWindow
{
  private readonly IUser32DllService _user32Dll;
  private readonly ISHCoreDllService _shCoreDll;

  public MainDialogWindow(IUser32DllService user32Dll,
                          ISHCoreDllService shCoreDll)
  {
    _user32Dll = user32Dll;
    _shCoreDll = shCoreDll;

    InitializeComponent();

    _hwnd = new WindowInteropHelper(this).EnsureHandle();
    HwndSource.FromHwnd(_hwnd).AddHook(HwndHook);
    _initialResizeMode = ResizeMode;
    _initialMargin = Margin;
  }


  private readonly nint _hwnd;
  private readonly ResizeMode _initialResizeMode;
  private readonly Thickness _initialMargin;


  public IDialogResult? Result { get; set; }


  private nint HwndHook(nint hWnd, int msg, nint wParam, nint lParam, ref bool handled)
  {
    switch (msg)
    {
      case 0x0112: // WM_SYSCOMMAND
        switch (wParam)
        {
          case 0xF030: // SC_MAXIMIZE
          case 0xF032: // SC_MAXIMIZE_DBLCLICK
            ResizeMode = ResizeMode.NoResize;
            break;
          case 0xF120: // SC_RESTORE
          case 0xF122: // SC_RESTORE_DBLCLICK
            ResizeMode = _initialResizeMode;
            break;
        }
        break;
    }
    return nint.Zero;
  }



  private void Window_StateChanged(object sender, EventArgs e)
  {
    if (WindowState == WindowState.Maximized)
    {
      var monitorHandle = _user32Dll.MonitorFromWindow(_hwnd, MonitorOptions.MONITOR_DEFAULTTONEAREST);
      if (_user32Dll.GetMonitorInfo(monitorHandle, out var monitorInfo)
          && _shCoreDll.GetDpiForMonitor(monitorHandle, MonitorDpiType.MDT_EFFECTIVE_DPI, out var dpiX, out var dpiY))
      {
        MaxWidth = (monitorInfo.WorkArea.Right - monitorInfo.WorkArea.Left) / (dpiX / 96d);
        MaxHeight = (monitorInfo.WorkArea.Bottom - monitorInfo.WorkArea.Top) / (dpiY / 96d);
        Margin = new(0, 0, 0, 0);
      }
    }
    else if (WindowState == WindowState.Normal)
    {
      Margin = _initialMargin;
    }
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
