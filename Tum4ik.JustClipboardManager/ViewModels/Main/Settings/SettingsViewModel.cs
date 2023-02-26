using CommunityToolkit.Mvvm.Input;
using Prism.Regions;
using Tum4ik.JustClipboardManager.Constants;

namespace Tum4ik.JustClipboardManager.ViewModels.Main.Settings;

internal partial class SettingsViewModel
{
  private readonly IRegionManager _regionManager;

  public SettingsViewModel(IRegionManager regionManager)
  {
    _regionManager = regionManager;
  }


  [RelayCommand]
  private void Navigate(string viewName)
  {
    _regionManager.RequestNavigate(RegionNames.MainDialogSettingsViewContent, viewName);
  }
}
