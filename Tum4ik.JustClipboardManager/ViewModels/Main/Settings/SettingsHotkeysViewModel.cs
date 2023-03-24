using CommunityToolkit.Mvvm.Input;
using Tum4ik.JustClipboardManager.Data.Models;
using Tum4ik.JustClipboardManager.Services;
using Tum4ik.JustClipboardManager.Services.Translation;
using Tum4ik.JustClipboardManager.ViewModels.Base;

namespace Tum4ik.JustClipboardManager.ViewModels.Main.Settings;

internal partial class SettingsHotkeysViewModel : TranslationViewModel
{
  public SettingsHotkeysViewModel(ITranslationService translationService,
                                  ISettingsService settingsService)
    : base(translationService)
  {
    Hotkeys.Add(new() { Description = "Show paste window", KeybindDescriptor = settingsService.HotkeyShowPasteWindow });
  }


  public List<Hotkey> Hotkeys { get; } = new();


  [RelayCommand]
  private void BindHotkey(Hotkey hotkey)
  {

  }
}
