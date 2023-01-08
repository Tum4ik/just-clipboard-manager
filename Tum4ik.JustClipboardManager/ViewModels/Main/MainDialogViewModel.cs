using CommunityToolkit.Mvvm.ComponentModel;
using Prism.Services.Dialogs;
using Tum4ik.JustClipboardManager.Services;

namespace Tum4ik.JustClipboardManager.ViewModels.Main;

[INotifyPropertyChanged]
internal partial class MainDialogViewModel : IDialogAware
{
  public MainDialogViewModel(IInfoService infoService)
  {
    Title = infoService.GetProductName();
  }


  public bool CanCloseDialog()
  {
    return true;
  }

  public void OnDialogClosed()
  {
  }

  public void OnDialogOpened(IDialogParameters parameters)
  {
  }

  public string Title { get; }
  public string? Tag { get; }
#if DEBUG
  = "Development";
#endif

  public event Action<IDialogResult>? RequestClose;
}
