using Tum4ik.JustClipboardManager.ViewModels;

namespace Tum4ik.JustClipboardManager.Views;
internal sealed partial class TrayIcon
{
  public TrayIcon(TrayIconViewModel vm)
  {
    DataContext = vm;
    InitializeComponent();
  }
}
