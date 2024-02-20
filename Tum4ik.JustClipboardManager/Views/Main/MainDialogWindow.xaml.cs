using System.Windows;
using System.Windows.Interop;
using Prism.Events;
using Prism.Services.Dialogs;
using Tum4ik.JustClipboardManager.Events;
using Tum4ik.JustClipboardManager.Helpers;
using Tum4ik.JustClipboardManager.Services.Dialogs;
using Tum4ik.JustClipboardManager.Services.PInvokeWrappers;
using Tum4ik.JustClipboardManager.Services.Theme;
using Windows.Win32;
using Windows.Win32.Graphics.Gdi;
using Windows.Win32.UI.HiDpi;
using Windows.Win32.UI.WindowsAndMessaging;

namespace Tum4ik.JustClipboardManager.Views.Main;

/// <summary>
/// Interaction logic for MainDialogWindow.xaml
/// </summary>
internal partial class MainDialogWindow : IDialogWindowExtended
{
  private readonly IThemeService _themeService;
  private readonly IUser32DllService _user32Dll;
  private readonly ISHCoreDllService _shCoreDll;

  public MainDialogWindow(IEventAggregator eventAggregator,
                          IThemeService themeService,
                          IUser32DllService user32Dll,
                          ISHCoreDllService shCoreDll)
  {
    _themeService = themeService;
    _user32Dll = user32Dll;
    _shCoreDll = shCoreDll;
    eventAggregator.GetEvent<ThemeChangedEvent>().Subscribe(OnThemeChanged);
    InitializeComponent();
  }


  public override void OnApplyTemplate()
  {
    base.OnApplyTemplate();
    WindowHelper.RemoveDefaultTitleBar(Handle);
    WindowHelper.ApplyBackdrop(Handle); // TODO: apply Mica backdrop from settings if allowed for OS
    WindowHelper.ApplyTheme(Handle, _themeService.SelectedTheme.ThemeType);
  }


  private nint? _handle;
  public nint Handle => _handle ??= new WindowInteropHelper(this).EnsureHandle();


  public IDialogResult? Result { get; set; }


  private void OnThemeChanged()
  {
    WindowHelper.ApplyTheme(Handle, _themeService.SelectedTheme.ThemeType);
  }


  private void Window_StateChanged(object sender, EventArgs e)
  {
    Padding = WindowState switch
    {
      WindowState.Maximized => GetPaddingForMaximizedWindow(),
      _ => default
    };
  }


  private readonly Dictionary<nint, Thickness> _monitorToMaximizedPadding = new();

  private Thickness GetPaddingForMaximizedWindow()
  {
    var monitorHandle = _user32Dll.MonitorFromWindow(Handle, MONITOR_FROM_FLAGS.MONITOR_DEFAULTTONEAREST);
    if (_monitorToMaximizedPadding.TryGetValue(monitorHandle, out var padding))
    {
      return padding;
    }

    if (_shCoreDll.GetDpiForMonitor(monitorHandle, MONITOR_DPI_TYPE.MDT_EFFECTIVE_DPI, out var dpiX, out var dpiY))
    {
      var paddedBorder = (double) PInvoke.GetSystemMetrics(SYSTEM_METRICS_INDEX.SM_CXPADDEDBORDER);
      var leftRight = SystemParameters.ResizeFrameVerticalBorderWidth + paddedBorder / (dpiX / 96d);
      var topBottom = SystemParameters.ResizeFrameHorizontalBorderHeight + paddedBorder / (dpiY / 96d);
      var thickness = new Thickness(leftRight, topBottom, leftRight, topBottom);
      _monitorToMaximizedPadding[monitorHandle] = thickness;
      return thickness;
    }

    return default;
  }
}
