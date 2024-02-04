using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Tum4ik.JustClipboardManager.Views;

namespace Tum4ik.JustClipboardManager.ViewModels;

internal sealed partial class TrayIconViewModel : ObservableObject
{
  private readonly Lazy<TrayMenuWindow> _trayMenu;

  public TrayIconViewModel(Lazy<TrayMenuWindow> trayMenu)
  {
    _trayMenu = trayMenu;
  }


  [ObservableProperty]
  private string _iconSource
#if DEBUG
    = "ms-appx:///Assets/tray-dev.ico";
#else
    = "ms-appx:///Assets/tray.ico";
#endif


  [RelayCommand]
  private void ShowMenu()
  {
    _trayMenu.Value.Activate();
  }
}
