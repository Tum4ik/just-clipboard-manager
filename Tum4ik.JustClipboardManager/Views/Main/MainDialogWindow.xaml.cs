using System.Windows.Interop;
using System.Windows.Media;
using Prism.Events;
using Prism.Services.Dialogs;
using Tum4ik.JustClipboardManager.Events;
using Tum4ik.JustClipboardManager.Services.Dialogs;
using Tum4ik.JustClipboardManager.Services.Theme;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;
using Windows.Win32.Graphics.Dwm;

namespace Tum4ik.JustClipboardManager.Views.Main;

/// <summary>
/// Interaction logic for MainDialogWindow.xaml
/// </summary>
internal partial class MainDialogWindow : IDialogWindowExtended
{
  private readonly IThemeService _themeService;

  public MainDialogWindow(IEventAggregator eventAggregator,
                          IThemeService themeService)
  {
    _themeService = themeService;
    eventAggregator.GetEvent<ThemeChangedEvent>().Subscribe(OnThemeChanged);
    InitializeComponent();
  }


  public override void OnApplyTemplate()
  {
    base.OnApplyTemplate();
    RemoveDefaultTitleBar();
    RemoveBackground();
    ApplyBackdrop(); // TODO: apply Mica backdrop from settings if allowed for OS
    ApplyTheme(_themeService.SelectedTheme.ThemeType);
  }


  private nint? _handle;
  public nint Handle => _handle ??= new WindowInteropHelper(this).EnsureHandle();


  public IDialogResult? Result { get; set; }


  private void OnThemeChanged()
  {
    ApplyTheme(_themeService.SelectedTheme.ThemeType);
  }


  private void RemoveDefaultTitleBar()
  {
    var hwnd = (HWND) Handle;
    var windowStyleLong = PInvoke.GetWindowLong(hwnd, WINDOW_LONG_PTR_INDEX.GWL_STYLE);
    windowStyleLong &= ~(int) WINDOW_STYLE.WS_SYSMENU;
    if (nint.Size == 4)
    {
      _ = PInvoke.SetWindowLong(hwnd, WINDOW_LONG_PTR_INDEX.GWL_STYLE, windowStyleLong);
    }
    else
    {
      PInvoke.SetWindowLongPtr(hwnd, WINDOW_LONG_PTR_INDEX.GWL_STYLE, windowStyleLong);
    }
  }


  private void RemoveBackground()
  {
    SetCurrentValue(BackgroundProperty, Brushes.Transparent);
    var windowSource = HwndSource.FromHwnd(Handle);
    if (windowSource?.Handle != IntPtr.Zero && windowSource?.CompositionTarget != null)
    {
      windowSource.CompositionTarget.BackgroundColor = Colors.Transparent;
    }
  }


  private unsafe void ApplyBackdrop()
  {
    if (Environment.OSVersion.Version >= new Version("10.0.22523"))
    {
      var backdropType = DWM_SYSTEMBACKDROP_TYPE.DWMSBT_MAINWINDOW;
      PInvoke.DwmSetWindowAttribute(
        (HWND) Handle,
        DWMWINDOWATTRIBUTE.DWMWA_SYSTEMBACKDROP_TYPE,
        &backdropType,
        sizeof(DWM_SYSTEMBACKDROP_TYPE)
      );
    }
    else
    {
      var backdropPvAttribute = 1;
      PInvoke.DwmSetWindowAttribute(
        (HWND) Handle,
        (DWMWINDOWATTRIBUTE) 1029,
        &backdropPvAttribute,
        sizeof(int)
      );
    }
  }


  private unsafe void ApplyTheme(ThemeType themeType)
  {
    var dwAttribute = Environment.OSVersion.Version >= new Version("10.0.22523")
      ? DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE
      : (DWMWINDOWATTRIBUTE) 19;
    var enableDark = themeType == ThemeType.Dark ? 0x1 : 0x0;
    PInvoke.DwmSetWindowAttribute((HWND) Handle, dwAttribute, &enableDark, sizeof(int));
  }
}
