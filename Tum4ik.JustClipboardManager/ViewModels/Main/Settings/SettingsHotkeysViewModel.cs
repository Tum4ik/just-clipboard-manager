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
                                  IDialogService dialogService)
    : base(translationService)
  {
    _dialogService = dialogService;

    Hotkeys.Add(new() { Description = "ShowPasteWindow", KeyBindingDescriptor = settingsService.HotkeyShowPasteWindow });
  }


  public List<Hotkey> Hotkeys { get; } = new();


  [RelayCommand]
  private void EditHotkey(Hotkey hotkey)
  {
    _dialogService.ShowDialog(DialogNames.EditHotkeyDialog);
  }
}
