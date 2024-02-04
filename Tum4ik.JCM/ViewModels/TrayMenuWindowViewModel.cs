using CommunityToolkit.Mvvm.Input;
using Tum4ik.JustClipboardManager.Services;
using Tum4ik.JustClipboardManager.Views.Main;

namespace Tum4ik.JustClipboardManager.ViewModels;
internal sealed partial class TrayMenuWindowViewModel
{
  private readonly IApplicationLifetime _applicationLifetime;
  private readonly Func<MainWindow> _mainWindow;

  public TrayMenuWindowViewModel(IApplicationLifetime applicationLifetime,
                                 Func<MainWindow> mainWindow)
  {
    _applicationLifetime = applicationLifetime;
    _mainWindow = mainWindow;
  }


  [RelayCommand]
  private void Settings()
  {
    _mainWindow().Activate();
  }


  [RelayCommand]
  private void About()
  {

  }


  [RelayCommand]
  private void Exit()
  {
    _applicationLifetime.ExitApplication();
  }
}
