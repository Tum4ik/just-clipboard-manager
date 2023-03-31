using Tum4ik.JustClipboardManager.Services;
using Tum4ik.JustClipboardManager.Services.Translation;
using Tum4ik.JustClipboardManager.ViewModels.Base;

namespace Tum4ik.JustClipboardManager.ViewModels.Main.Settings;
internal class SettingsGeneralViewModel : TranslationViewModel
{
  private readonly IShortcutService _shortcutService;

  public SettingsGeneralViewModel(ITranslationService translationService,
                                  IShortcutService shortcutService)
    : base(translationService)
  {
    _shortcutService = shortcutService;
  }


  public bool AutoStartApplication
  {
    get => _shortcutService.Exists(Environment.SpecialFolder.Startup, out _);
    set
    {
      if (value)
      {
        _shortcutService.Create(Environment.SpecialFolder.Startup);
      }
      else
      {
        _shortcutService.Delete(Environment.SpecialFolder.Startup);
      }
      OnPropertyChanged();
    }
  }
}
