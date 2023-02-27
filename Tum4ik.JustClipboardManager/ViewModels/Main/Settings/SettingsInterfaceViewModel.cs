using CommunityToolkit.Mvvm.ComponentModel;
using Prism.Events;
using Tum4ik.JustClipboardManager.Services;
using Tum4ik.JustClipboardManager.Services.Translation;
using Tum4ik.JustClipboardManager.ViewModels.Base;

namespace Tum4ik.JustClipboardManager.ViewModels.Main.Settings;
internal partial class SettingsInterfaceViewModel : TranslationViewModel
{
  private readonly ITranslationService _translationService;
  private readonly ISettingsService _settingsService;

  public SettingsInterfaceViewModel(ITranslationService translationService,
                                    IEventAggregator eventAggregator,
                                    ISettingsService settingsService)
    : base(translationService, eventAggregator)
  {
    _translationService = translationService;
    _settingsService = settingsService;

    Languages = _translationService.SupportedLanguages;
    SelectedLanguage = Languages.SingleOrDefault(l => l.Culture.Equals(_settingsService.Language));
  }


  public Language[] Languages { get; }


  [ObservableProperty] private Language? _selectedLanguage;
  partial void OnSelectedLanguageChanged(Language? value)
  {
    if (value is not null)
    {
      _settingsService.Language = value.Culture;
    }
  }
}
