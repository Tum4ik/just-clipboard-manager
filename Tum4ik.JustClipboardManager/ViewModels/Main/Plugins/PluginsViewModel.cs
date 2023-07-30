using CommunityToolkit.Mvvm.Input;
using Prism.Events;
using Prism.Regions;
using Tum4ik.JustClipboardManager.Constants;
using Tum4ik.JustClipboardManager.Services.Translation;
using Tum4ik.JustClipboardManager.ViewModels.Base;

namespace Tum4ik.JustClipboardManager.ViewModels.Main.Plugins;
internal partial class PluginsViewModel : TranslationViewModel
{
  private readonly IRegionManager _regionManager;

  public PluginsViewModel(ITranslationService translationService,
                          IEventAggregator eventAggregator,
                          IRegionManager regionManager)
    : base(translationService, eventAggregator)
  {
    _regionManager = regionManager;
  }


  [RelayCommand]
  private void Navigate(string viewName)
  {
    _regionManager.RequestNavigate(RegionNames.MainDialogPluginsViewContent, viewName);
  }
}
