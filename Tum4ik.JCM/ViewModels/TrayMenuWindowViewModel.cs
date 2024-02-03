using CommunityToolkit.Mvvm.Input;
using Tum4ik.JustClipboardManager.Services;

namespace Tum4ik.JustClipboardManager.ViewModels;
internal partial class TrayMenuWindowViewModel
{
  private readonly IApplicationLifetime _applicationLifetime;

  public TrayMenuWindowViewModel(IApplicationLifetime applicationLifetime)
  {
    _applicationLifetime = applicationLifetime;
  }


  [RelayCommand]
  private void Exit()
  {
    _applicationLifetime.ExitApplication();
  }
}
