using CommunityToolkit.Mvvm.ComponentModel;
using Prism.Services.Dialogs;
using Tum4ik.JustClipboardManager.Constants;
using Tum4ik.JustClipboardManager.Services;

namespace Tum4ik.JustClipboardManager.ViewModels.Main;

internal partial class MainDialogViewModel : ObservableObject, IDialogAware
{
  public MainDialogViewModel(IInfoService infoService)
  {
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
  public string? Tag { get; }
#if DEBUG
  = "Development";
#endif

  public event Action<IDialogResult>? RequestClose;
}
