using CommunityToolkit.Mvvm.Input;
using Prism.Services.Dialogs;
using Tum4ik.JustClipboardManager.Services.Dialogs;
using Tum4ik.JustClipboardManager.Services.Translation;
using Tum4ik.JustClipboardManager.ViewModels.Base;

namespace Tum4ik.JustClipboardManager.ViewModels.Main.Settings;
internal partial class EditHotkeyDialogViewModel : TranslationViewModel, ISimpleDialogAware
{
  public EditHotkeyDialogViewModel(ITranslationService translationService) : base(translationService)
  {
  }


  public string? Title { get; } = "EditHotkey";
  public string? CancelButtonText { get; } = "Cancel";
  public string? AcceptButtonText { get; } = "Accept";

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


  [RelayCommand]
  private void CancelButtonPressed()
  {
    RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
  }


  [RelayCommand]
  private void AcceptButtonPressed()
  {

  }
}
