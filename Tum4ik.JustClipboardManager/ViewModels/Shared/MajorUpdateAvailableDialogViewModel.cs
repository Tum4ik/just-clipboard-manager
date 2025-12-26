using System.Diagnostics;
using CommunityToolkit.Mvvm.Input;
using Prism.Events;
using Prism.Services.Dialogs;
using Tum4ik.JustClipboardManager.Services.Dialogs;
using Tum4ik.JustClipboardManager.Services.Translation;
using Tum4ik.JustClipboardManager.ViewModels.Base;

namespace Tum4ik.JustClipboardManager.ViewModels.Shared;
internal partial class MajorUpdateAvailableDialogViewModel : TranslationViewModel, ISimpleDialogAware
{
  public MajorUpdateAvailableDialogViewModel(ITranslationService translationService, IEventAggregator eventAggregator)
    : base(translationService, eventAggregator)
  {
  }

  [RelayCommand]
  private void CancelButtonPressed()
  {
    RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
  }

  public IRelayCommand? AcceptButtonPressedCommand { get; }
  public string? CancelButtonText { get; } = "Ok";
  public string? AcceptButtonText { get; }

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

  public string Title { get; } = "MajorUpdateAvailableDialogTitle";

  public event Action<IDialogResult> RequestClose;

  [RelayCommand]
  private void OpenLink(string? link)
  {
    if (string.IsNullOrWhiteSpace(link))
    {
      return;
    }
    Process.Start(new ProcessStartInfo(link) { UseShellExecute = true });
  }
}
