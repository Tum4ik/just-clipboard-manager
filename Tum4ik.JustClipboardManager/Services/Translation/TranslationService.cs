using System.Resources;
using CommunityToolkit.Mvvm.ComponentModel;
using Tum4ik.JustClipboardManager.Icons;

namespace Tum4ik.JustClipboardManager.Services.Translation;

[INotifyPropertyChanged]
internal partial class TranslationService : ResourceManager, ITranslationService
{
  private readonly ISettingsService _settingsService;

  public TranslationService(ISettingsService settingsService)
    : base(typeof(Resources.Translations.Translation))
  {
    _settingsService = settingsService;

    _selectedLanguage
      = SupportedLanguages.SingleOrDefault(l => l.Culture.Equals(settingsService.Language))
      ?? SupportedLanguages.First();
  }


  public string this[string key]
  {
    get
    {
      return GetString(key, _settingsService.Language) ?? key;
    }
  }


  public IEnumerable<Language> SupportedLanguages { get; } = new[]
  {
    new Language(new("en-US"), SvgIconType.USA),
    new Language(new("uk-UA"), SvgIconType.Ukraine)
  };


  [ObservableProperty] private Language _selectedLanguage;
  partial void OnSelectedLanguageChanged(Language value)
  {
    _settingsService.Language = value.Culture;
    LanguageChanged?.Invoke();
  }


  public event Action? LanguageChanged;
}
