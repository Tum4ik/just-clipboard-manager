using Tum4ik.JustClipboardManager.ViewModels;

namespace Tum4ik.JustClipboardManager.Views;
internal sealed partial class TrayIcon
{
  public TrayIconViewModel Vm { get; }

  public TrayIcon(TrayIconViewModel vm)
  {
    Vm = vm;
    DataContext = vm;
    InitializeComponent();
  }
}
