using System.Collections.Immutable;
using Prism.Events;
using Tum4ik.JustClipboardManager.Events;
using Tum4ik.JustClipboardManager.Services.Theme;
using Tum4ik.JustClipboardManager.Services.Translation;
using Tum4ik.JustClipboardManager.ViewModels.Base;

namespace Tum4ik.JustClipboardManager.ViewModels.Main.Settings;
internal partial class SettingsInterfaceViewModel : TranslationViewModel
{
  private readonly IThemeService _themeService;

  public SettingsInterfaceViewModel(ITranslationService translationService,
                                    IThemeService themeService,
                                    IEventAggregator eventAggregator)
    : base(translationService)
  {
    _themeService = themeService;

    eventAggregator.GetEvent<ThemeChangedEvent>().Subscribe(() => OnPropertyChanged(nameof(SelectedTheme)));
  }


  public ImmutableArray<ColorTheme> Themes => _themeService.Themes;


  public ColorTheme SelectedTheme
  {
    get => _themeService.SelectedTheme;
    set => _themeService.SelectedTheme = value;
  }
}
