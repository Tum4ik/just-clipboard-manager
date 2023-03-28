using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Prism.Services.Dialogs;
using Tum4ik.JustClipboardManager.Data.Models;
using Tum4ik.JustClipboardManager.Services.Dialogs;
using Tum4ik.JustClipboardManager.Services.Translation;
using Tum4ik.JustClipboardManager.ViewModels.Base;

namespace Tum4ik.JustClipboardManager.ViewModels.Main.Settings;
internal partial class EditHotkeyDialogViewModel : TranslationViewModel, ISimpleDialogAware
{
  public EditHotkeyDialogViewModel(ITranslationService translationService) : base(translationService)
  {
  }


  private ModifierKeys _pressedModifiers;
  private Key _pressedKey;


  public string? Title { get; } = "EditHotkey";
  public string? CancelButtonText { get; } = "Cancel";
  public string? AcceptButtonText { get; } = "Accept";

  public event Action<IDialogResult>? RequestClose;


  [ObservableProperty] private KeyBindingDescriptor _keyBindingDescriptor = new(ModifierKeys.None, Key.None);


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
    switch (args.Key)
    {
      case Key.LeftAlt:
      case Key.RightAlt:
        AddModifierKey(ModifierKeys.Alt);
        break;
      case Key.LeftCtrl:
      case Key.RightCtrl:
        AddModifierKey(ModifierKeys.Control);
        break;
      case Key.LeftShift:
      case Key.RightShift:
        AddModifierKey(ModifierKeys.Shift);
        break;
      case Key.LWin:
      case Key.RWin:
        AddModifierKey(ModifierKeys.Windows);
        break;
      default:
        if (_pressedKey == Key.None)
        {
          _pressedKey = args.Key;
        }
        break;
    }
    UpdateKeyBindingDescriptor();
  }


  [RelayCommand]
  private void KeyboardKeyUp(KeyEventArgs args)
  {
    if (_pressedModifiers == ModifierKeys.None || _pressedKey == Key.None)
    {
      switch (args.Key)
      {
        case Key.LeftAlt:
        case Key.RightAlt:
          RemoveModifierKey(ModifierKeys.Alt);
          break;
        case Key.LeftCtrl:
        case Key.RightCtrl:
          RemoveModifierKey(ModifierKeys.Control);
          break;
        case Key.LeftShift:
        case Key.RightShift:
          RemoveModifierKey(ModifierKeys.Shift);
          break;
        case Key.LWin:
        case Key.RWin:
          RemoveModifierKey(ModifierKeys.Windows);
          break;
        default:
          if (_pressedKey == args.Key)
          {
            _pressedKey = Key.None;
          }
          break;
      }
      UpdateKeyBindingDescriptor();
    }
    else if (_pressedKey == args.Key)
    {

    }
  }


  private void AddModifierKey(ModifierKeys modifier)
  {
    if (!_pressedModifiers.HasFlag(modifier))
    {
      _pressedModifiers |= modifier;
    }
  }


  private void RemoveModifierKey(ModifierKeys modifier)
  {
    if (_pressedModifiers.HasFlag(modifier))
    {
      _pressedModifiers &= ~modifier;
    }
  }


  private void UpdateKeyBindingDescriptor()
  {
    KeyBindingDescriptor = new(_pressedModifiers, _pressedKey);
  }
}
