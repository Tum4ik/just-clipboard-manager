using Tum4ik.JustClipboardManager.Mvvm;
using Tum4ik.JustClipboardManager.Services;
using Tum4ik.JustClipboardManager.ViewModels.Main;

namespace Tum4ik.JustClipboardManager.Views.Main;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
internal sealed partial class AboutPage : IPageVmAware<AboutPageViewModel>
{
  public AboutPageViewModel Vm { get; }

  public AboutPage()
  {
    Vm = ServiceLocator.Current.Resolve<AboutPageViewModel>();
    InitializeComponent();
  }
}
