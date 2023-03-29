using CommunityToolkit.Mvvm.Input;
using Prism.Services.Dialogs;
using Tum4ik.JustClipboardManager.Services.Dialogs;
using Tum4ik.JustClipboardManager.Services.Translation;
using Tum4ik.JustClipboardManager.ViewModels.Base;

namespace Tum4ik.JustClipboardManager.ViewModels.Shared;
internal partial class UnregisteredHotkeysDialogViewModel : TranslationViewModel, ISimpleDialogAware
{
  public UnregisteredHotkeysDialogViewModel(ITranslationService translationService)
    : base(translationService)
  {
  }


  public string? Title { get; } = "UndefinedHotkeysDetectedTitle";
  public string? CancelButtonText { get; } = "Ok";
  public string? AcceptButtonText { get; }
  public event Action<IDialogResult>? RequestClose;


  public void OnDialogOpened(IDialogParameters parameters)
  {
  }


  public bool CanCloseDialog()
  {
    return true;
  }


  public void OnDialogClosed()
  {
  }


  public IRelayCommand? AcceptButtonPressedCommand { get; }


  [RelayCommand]
  private void CancelButtonPressed()
  {
    RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
  }
}
