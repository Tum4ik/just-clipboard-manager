using Tum4ik.JustClipboardManager.Mvvm;
using Tum4ik.JustClipboardManager.Services;
using Tum4ik.JustClipboardManager.ViewModels.Main.Plugins;

namespace Tum4ik.JustClipboardManager.Views.Main.Plugins;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
internal sealed partial class PluginsPage : IPageVmAware<PluginsPageViewModel>
{
  public PluginsPageViewModel Vm { get; }

  public PluginsPage()
  {
    Vm = ServiceLocator.Current.Resolve<PluginsPageViewModel>();
    InitializeComponent();
  }
}
