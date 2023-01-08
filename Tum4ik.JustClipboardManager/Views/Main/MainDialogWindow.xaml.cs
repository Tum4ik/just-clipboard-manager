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
    _initialMargin = Margin;
  }


  private readonly nint _hwnd;
  private readonly Thickness _initialMargin;


  public IDialogResult? Result { get; set; }

  
  private void Window_StateChanged(object sender, EventArgs e)
  {
    if (WindowState == WindowState.Maximized)
    {
      var monitorHandle = _user32Dll.MonitorFromWindow(_hwnd, MonitorOptions.MONITOR_DEFAULTTONEAREST);
      if (_user32Dll.GetMonitorInfo(monitorHandle, out var monitorInfo)
          && _shCoreDll.GetDpiForMonitor(monitorHandle, MonitorDpiType.MDT_EFFECTIVE_DPI, out var dpiX, out var dpiY))
      {
        var compensationMarginX = 7 / (dpiX / 96d);
        var compensationMarginY = 7 / (dpiY / 96d);
        MaxWidth = (monitorInfo.WorkArea.Right - monitorInfo.WorkArea.Left) / (dpiX / 96d) + compensationMarginX;
        MaxHeight = (monitorInfo.WorkArea.Bottom - monitorInfo.WorkArea.Top) / (dpiY / 96d) + compensationMarginY;
        Margin = new(compensationMarginX, compensationMarginY, 0, 0);
      }
    }
    else if (WindowState == WindowState.Normal)
    {
      Margin = _initialMargin;
    }
  }


  private void MinimizeButton_Click(object sender, RoutedEventArgs e)
  {
    WindowState = WindowState.Minimized;
  }


  private void MaximizeRestoreButton_Click(object sender, RoutedEventArgs e)
  {
    if (WindowState == WindowState.Normal)
    {
      WindowState = WindowState.Maximized;
    }
    else if (WindowState == WindowState.Maximized)
    {
      WindowState = WindowState.Normal;
    }
  }


  private void CloseButton_Click(object sender, RoutedEventArgs e)
  {
    Result = new DialogResult();
    Close();
  }
}
