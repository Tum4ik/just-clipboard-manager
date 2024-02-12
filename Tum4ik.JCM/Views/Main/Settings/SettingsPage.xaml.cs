using Tum4ik.JustClipboardManager.Mvvm;
using Tum4ik.JustClipboardManager.Services;
using Tum4ik.JustClipboardManager.ViewModels.Main.Settings;

namespace Tum4ik.JustClipboardManager.Views.Main.Settings;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
internal sealed partial class SettingsPage : IPageVmAware<SettingsPageViewModel>
{
  public SettingsPageViewModel Vm { get; }

  public SettingsPage()
  {
    Vm = ServiceLocator.Current.Resolve<SettingsPageViewModel>();
    InitializeComponent();
  }
}
