using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using Tum4ik.JustClipboardManager.ViewModels.Main.Settings;

namespace Tum4ik.JustClipboardManager.Views.Main.Settings;
/// <summary>
/// Interaction logic for EditHotkeyDialog.xaml
/// </summary>
public partial class EditHotkeyDialog
{
  public EditHotkeyDialog()
  {
    InitializeComponent();

    Loaded += (s, e) =>
    {
      var window = Window.GetWindow(this);
      var handle = new WindowInteropHelper(window).EnsureHandle();
      _hwndSource = HwndSource.FromHwnd(handle);
      _hwndSource.AddHook(HwndHook);
      _viewModel = DataContext as EditHotkeyDialogViewModel;
    };
    Unloaded += (s, e) =>
    {
      _hwndSource?.RemoveHook(HwndHook);
    };
  }


  private HwndSource? _hwndSource;
  private EditHotkeyDialogViewModel? _viewModel;


  private nint HwndHook(nint hWnd, int msg, nint wParam, nint lParam, ref bool handled)
  {
    switch (msg)
    {
      case 0x0100: // WM_KEYDOWN
      case 0x0104: // WM_SYSKEYDOWN
      {
        var key = KeyInterop.KeyFromVirtualKey(wParam.ToInt32());
        _viewModel?.KeyboardKeyDownCommand.Execute(key);
        break;
      }
      case 0x0101: // WM_KEYUP
      case 0x0105: // WM_SYSKEYUP
      {
        var key = KeyInterop.KeyFromVirtualKey(wParam.ToInt32());
        _viewModel?.KeyboardKeyUpCommand.Execute(key);
        break;
      }
    }

    return nint.Zero;
  }
}
