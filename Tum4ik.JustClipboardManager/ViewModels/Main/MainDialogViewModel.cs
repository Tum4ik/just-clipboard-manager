using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Prism.Events;
using Prism.Regions;
using Prism.Services.Dialogs;
using Tum4ik.JustClipboardManager.Constants;
using Tum4ik.JustClipboardManager.Services;
using Tum4ik.JustClipboardManager.Services.Translation;
using Tum4ik.JustClipboardManager.ViewModels.Base;

namespace Tum4ik.JustClipboardManager.ViewModels.Main;

internal partial class MainDialogViewModel : TranslationViewModel, IDialogAware
{
  private readonly IRegionManager _regionManager;

  public MainDialogViewModel(IInfoService infoService,
                             IRegionManager regionManager,
                             ITranslationService translationService,
                             IEventAggregator eventAggregator)
    : base(translationService, eventAggregator)
  {
    _regionManager = regionManager;

    Title = infoService.GetProductName();
  }


  [ObservableProperty] private bool _isSettingsTabChecked;
  [ObservableProperty] private bool _isAboutTabChecked;
  

  public bool CanCloseDialog()
  {
    return true;
  }

  public void OnDialogClosed()
  {
    var names = new List<string>();
    foreach (var region in _regionManager.Regions)
    {
      region.RemoveAll();
      names.Add(region.Name);
    }
    foreach (var name in names)
    {
      _regionManager.Regions.Remove(name);
    }
  }

  public void OnDialogOpened(IDialogParameters parameters)
  {
    if (parameters.TryGetValue(DialogParameterNames.ViewToShow, out string viewName))
    {
      switch (viewName)
      {
        default:
          IsSettingsTabChecked = true;
          break;
        case ViewNames.AboutView:
          IsAboutTabChecked = true;
          break;
      }
    }
  }

  public string Title { get; }
  public event Action<IDialogResult>? RequestClose;


  [RelayCommand]
  private void Navigate(string viewName)
  {
    _regionManager.RequestNavigate(RegionNames.MainDialogContent, viewName);
  }
}
