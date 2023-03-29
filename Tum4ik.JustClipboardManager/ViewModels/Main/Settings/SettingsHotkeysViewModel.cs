using CommunityToolkit.Mvvm.Input;
using Prism.Services.Dialogs;
using Tum4ik.JustClipboardManager.Constants;
using Tum4ik.JustClipboardManager.Data.Models;
using Tum4ik.JustClipboardManager.Services;
using Tum4ik.JustClipboardManager.Services.Translation;
using Tum4ik.JustClipboardManager.ViewModels.Base;

namespace Tum4ik.JustClipboardManager.ViewModels.Main.Settings;

internal partial class SettingsHotkeysViewModel : TranslationViewModel
{
  private readonly IDialogService _dialogService;

  public SettingsHotkeysViewModel(ITranslationService translationService,
                                  ISettingsService settingsService,
                                  IDialogService dialogService,
                                  IKeyboardHookService keyboardHookService)
    : base(translationService)
  {
    _dialogService = dialogService;

    Hotkeys.Add(
      new(
        "ShowPasteWindow",
        settingsService.HotkeyShowPasteWindow,
        keyboardHookService.RegisterShowPasteWindowHotkey
      )
    );
  }


  public List<Hotkey> Hotkeys { get; } = new();


  [RelayCommand]
  private void EditHotkey(Hotkey hotkey)
  {
    var parameters = new DialogParameters
    {
      { DialogParameterNames.KeyBindingDescriptor, hotkey.KeyBindingDescriptor },
      { DialogParameterNames.HotkeyRegisterAction, hotkey.RegisterAction }
    };
    _dialogService.ShowDialog(DialogNames.EditHotkeyDialog, parameters, r =>
    {
      if (r.Result == ButtonResult.OK
        && r.Parameters.TryGetValue(DialogParameterNames.KeyBindingDescriptor, out KeyBindingDescriptor descriptor))
      {
        hotkey.KeyBindingDescriptor = descriptor;
      }
    });
  }
}
