using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Prism.Services.Dialogs;
using Tum4ik.JustClipboardManager.Data.Models;
using Tum4ik.JustClipboardManager.Services;
using Tum4ik.JustClipboardManager.Services.Dialogs;
using Tum4ik.JustClipboardManager.Services.Translation;
using Tum4ik.JustClipboardManager.ViewModels.Base;

namespace Tum4ik.JustClipboardManager.ViewModels.Main.Settings;
internal partial class EditHotkeyDialogViewModel : TranslationViewModel, ISimpleDialogAware
{
  public EditHotkeyDialogViewModel(ITranslationService translationService,
                                   IKeyBindingRecordingService keyBindingRecordingService)
    : base(translationService)
  {
    _keyBindingRecordingService = keyBindingRecordingService;
  }


  public string? Title { get; } = "EditHotkey";
  public string? CancelButtonText { get; } = "Cancel";
  public string? AcceptButtonText { get; } = "Accept";

  public event Action<IDialogResult>? RequestClose;


  [ObservableProperty] private KeyBindingDescriptor _keyBindingDescriptor = new(ModifierKeys.None, Key.None);
  private readonly IKeyBindingRecordingService _keyBindingRecordingService;

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


  [RelayCommand]
  private void KeyboardKeyDown(KeyEventArgs args)
  {
    (KeyBindingDescriptor, _) = _keyBindingRecordingService.RecordKeyDown(args.Key);
  }


  [RelayCommand]
  private void KeyboardKeyUp(KeyEventArgs args)
  {
    (KeyBindingDescriptor, _) = _keyBindingRecordingService.RecordKeyUp(args.Key);
  }
}
