using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Prism.Events;
using Prism.Services.Dialogs;
using Tum4ik.JustClipboardManager.Constants;
using Tum4ik.JustClipboardManager.PluginDevKit.Models;
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
                                   IEventAggregator eventAggregator,
                                   IKeyBindingRecordingService keyBindingRecordingService,
                                   IKeyboardHookService keyboardHookService)
    : base(translationService, eventAggregator)
  {
    _keyBindingRecordingService = keyBindingRecordingService;
    _keyboardHookService = keyboardHookService;
  }


  public string? Title { get; } = "EditHotkey";
  public string? CancelButtonText { get; } = "Cancel";
  public string? AcceptButtonText { get; } = "Accept";

  public event Action<IDialogResult>? RequestClose;


  [ObservableProperty] private KeyBindingDescriptor _keyBindingDescriptor = new(ModifierKeys.None, Key.None);
  [ObservableProperty, NotifyPropertyChangedFor(nameof(ErrorMessageVisibility))] private string? _errorMessage;
  public Visibility ErrorMessageVisibility => string.IsNullOrEmpty(ErrorMessage) ? Visibility.Collapsed : Visibility.Visible;
  [ObservableProperty, NotifyCanExecuteChangedFor(nameof(AcceptButtonPressedCommand))] private bool _canAcceptHotkey;


  private KeyBindingDescriptor? _previousDescriptor;
  private Func<KeyBindingDescriptor, bool>? _hotkeyRegisterAction;
  private HashSet<KeyBindingDescriptor> _disallowedHotkeys = new()
  {
    new(ModifierKeys.Control, Key.X),
    new(ModifierKeys.Control, Key.C),
    new(ModifierKeys.Control, Key.V)
  };


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

    if (_disallowedHotkeys.Contains(KeyBindingDescriptor))
    {
      ErrorMessage = "DisallowedHotkey";
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

    ErrorMessage = "HotkeyIsAlreadyRegistered";
  }


  public void KeyboardKeyDown(Key key)
  {
    ErrorMessage = null;
    (KeyBindingDescriptor, CanAcceptHotkey) = _keyBindingRecordingService.RecordKeyDown(key);
  }


  public void KeyboardKeyUp(Key key)
  {
    ErrorMessage = null;
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
    ErrorMessage = null;
    (KeyBindingDescriptor, CanAcceptHotkey) = _keyBindingRecordingService.ResetRecord();
  }
}
