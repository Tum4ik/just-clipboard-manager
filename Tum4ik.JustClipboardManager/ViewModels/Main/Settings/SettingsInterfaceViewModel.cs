using Tum4ik.JustClipboardManager.Services.Theme;
using Tum4ik.JustClipboardManager.Services.Translation;
using Tum4ik.JustClipboardManager.ViewModels.Base;

namespace Tum4ik.JustClipboardManager.ViewModels.Main.Settings;
internal partial class SettingsInterfaceViewModel : TranslationViewModel
{
  public IThemeService ThemeService { get; }

  public SettingsInterfaceViewModel(ITranslationService translationService,
                                    IThemeService themeService)
    : base(translationService)
  {
    ThemeService = themeService;
  }
}
