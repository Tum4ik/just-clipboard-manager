using CommunityToolkit.Mvvm.Input;
using Prism.Regions;
using Tum4ik.JustClipboardManager.Constants;
using Tum4ik.JustClipboardManager.Services.Translation;
using Tum4ik.JustClipboardManager.ViewModels.Base;

namespace Tum4ik.JustClipboardManager.ViewModels.Main.Settings;

internal partial class SettingsViewModel : TranslationViewModel
{
  private readonly IRegionManager _regionManager;

  public SettingsViewModel(IRegionManager regionManager,
                           ITranslationService translationService)
    : base(translationService)
  {
    _regionManager = regionManager;
  }


  [RelayCommand]
  private void Navigate(string viewName)
  {
    _regionManager.RequestNavigate(RegionNames.MainDialogSettingsViewContent, viewName);
  }
}
