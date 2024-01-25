using CommunityToolkit.Mvvm.ComponentModel;

namespace Tum4ik.JustClipboardManager.ViewModels;

internal partial class TrayIconViewModel : ObservableObject
{

  public TrayIconViewModel()
  {
  }


  [ObservableProperty]
  private string _iconSource
#if DEBUG
    = "ms-appx:///Assets/tray-dev.ico";
#else
    = "ms-appx:///Assets/Icons/tray.ico";
#endif
}
