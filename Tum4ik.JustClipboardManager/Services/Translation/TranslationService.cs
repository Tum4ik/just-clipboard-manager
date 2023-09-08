using System.Collections.Immutable;
using System.Resources;
using Prism.Events;
using Tum4ik.JustClipboardManager.Events;
using Tum4ik.JustClipboardManager.Resources.Icons;

namespace Tum4ik.JustClipboardManager.Services.Translation;

internal class TranslationService : ResourceManager, ITranslationService
{
  private readonly ISettingsService _settingsService;
  private readonly IEventAggregator _eventAggregator;

  public TranslationService(ISettingsService settingsService,
                            IEventAggregator eventAggregator)
    : base(typeof(Resources.Translations.Translation))
  {
    _settingsService = settingsService;
    _eventAggregator = eventAggregator;
  }


  public string this[string key]
  {
    get
    {
      return GetString(key, _settingsService.Language) ?? key;
    }
  }


  public ImmutableArray<Language> SupportedLanguages { get; } = new[]
  {
    new Language(new("en-US"), SvgIconType.USA),
    new Language(new("uk-UA"), SvgIconType.Ukraine)
  }.ToImmutableArray();


  private Language? _selectedLanguage;
  public Language SelectedLanguage
  {
    get => _selectedLanguage ??= GetSelectedLanguage();
    set
    {
      if (_selectedLanguage != value)
      {
        _selectedLanguage = value;
        _settingsService.Language = value.Culture;
        _eventAggregator.GetEvent<LanguageChangedEvent>().Publish();
      }
    }
  }


  private Language GetSelectedLanguage()
  {
    return SupportedLanguages.SingleOrDefault(l => l.Culture.Equals(_settingsService.Language))
      ?? SupportedLanguages.First();
  }
}
