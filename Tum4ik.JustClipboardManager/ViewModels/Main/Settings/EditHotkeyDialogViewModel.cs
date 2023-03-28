using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Prism.Services.Dialogs;
using Tum4ik.JustClipboardManager.Constants;
using Tum4ik.JustClipboardManager.Data.Models;
using Tum4ik.JustClipboardManager.Services;
using Tum4ik.JustClipboardManager.Services.Dialogs;
using Tum4ik.JustClipboardManager.Services.Translation;
using Tum4ik.JustClipboardManager.ViewModels.Base;

namespace Tum4ik.JustClipboardManager.ViewModels.Main.Settings;
internal partial class EditHotkeyDialogViewModel : TranslationViewModel, ISimpleDialogAware
{
  private readonly IKeyBindingRecordingService _keyBindingRecordingService;
  private readonly IKeyboardHookService _keyboardHookService;

  public EditHotkeyDialogViewModel(ITranslationService translationService,
                                   IKeyBindingRecordingService keyBindingRecordingService,
                                   IKeyboardHookService keyboardHookService)
    : base(translationService)
  {
    _keyBindingRecordingService = keyBindingRecordingService;
    _keyboardHookService = keyboardHookService;
  }


  public string? Title { get; } = "EditHotkey";
  public string? CancelButtonText { get; } = "Cancel";
  public string? AcceptButtonText { get; } = "Accept";

  public event Action<IDialogResult>? RequestClose;


  [ObservableProperty] private KeyBindingDescriptor _keyBindingDescriptor = new(ModifierKeys.None, Key.None);
  [ObservableProperty] private Visibility _errorMessageVisibility = Visibility.Collapsed;
  [ObservableProperty, NotifyCanExecuteChangedFor(nameof(AcceptButtonPressedCommand))] private bool _canAcceptHotkey;


  private KeyBindingDescriptor? _previousDescriptor;
  private Func<KeyBindingDescriptor, bool>? _hotkeyRegisterAction;


  public void OnDialogOpened(IDialogParameters parameters)
  {
    if (parameters.TryGetValue(DialogParameterNames.KeyBindingDescriptor, out KeyBindingDescriptor descriptor)
      && parameters.TryGetValue(DialogParameterNames.HotkeyRegisterAction, out Func<KeyBindingDescriptor, bool> registerAction))
    {
      _previousDescriptor = descriptor;
      _hotkeyRegisterAction = registerAction;
    }
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


  [RelayCommand(CanExecute = nameof(CanAcceptHotkey))]
  private void AcceptButtonPressed()
  {
    if (_previousDescriptor == KeyBindingDescriptor)
    {
      RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
      return;
    }

    if ((_hotkeyRegisterAction?.Invoke(KeyBindingDescriptor) ?? false)
      && _previousDescriptor is not null)
    {
      _keyboardHookService.UnregisterHotKey(_previousDescriptor);
      var parameters = new DialogParameters
      {
        { DialogParameterNames.KeyBindingDescriptor, KeyBindingDescriptor }
      };
      RequestClose?.Invoke(new DialogResult(ButtonResult.OK, parameters));
      return;
    }

    ErrorMessageVisibility = Visibility.Visible;
  }


  public void KeyboardKeyDown(Key key)
  {
    ErrorMessageVisibility = Visibility.Collapsed;
    (KeyBindingDescriptor, CanAcceptHotkey) = _keyBindingRecordingService.RecordKeyDown(key);
  }


  public void KeyboardKeyUp(Key key)
  {
    ErrorMessageVisibility = Visibility.Collapsed;
    (KeyBindingDescriptor, CanAcceptHotkey) = _keyBindingRecordingService.RecordKeyUp(key);
  }


  public void DialogDeactivated()
  {
    if (!_keyBindingRecordingService.Completed)
    {
      (KeyBindingDescriptor, CanAcceptHotkey) = _keyBindingRecordingService.ResetRecord();
    }
  }


  [RelayCommand]
  private void ResetHotkey()
  {
    ErrorMessageVisibility = Visibility.Collapsed;
    (KeyBindingDescriptor, CanAcceptHotkey) = _keyBindingRecordingService.ResetRecord();
  }
}
