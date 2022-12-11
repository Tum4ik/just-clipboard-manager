using System.Windows;

namespace Tum4ik.JustClipboardManager.Views;
/// <summary>
/// Interaction logic for TrayIcon.xaml
/// </summary>
public partial class TrayIcon
{
  public TrayIcon()
  {
    InitializeComponent();
    ContextMenu.Resources = Application.Current.Resources;
  }
}
