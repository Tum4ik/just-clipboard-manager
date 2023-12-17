using System.Collections.Immutable;
using System.Resources;
using Prism.Events;
using Tum4ik.JustClipboardManager.PluginDevKit.Events;
using Tum4ik.JustClipboardManager.PluginDevKit.Services;
using Tum4ik.JustClipboardManager.Resources.Icons;

namespace Tum4ik.JustClipboardManager.Services.Translation;

internal class TranslationService : ResourceManager, ITranslationService
{
  private const string SingularSuffix = "_Singular";
  private const string PaucalSuffix = "_Paucal";
  private const string PluralSuffix = "_Plural";

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


  public string this[string key, int quantity]
  {
    get
    {
      var suffix = PluralSuffix;
      quantity = Math.Abs(quantity);
      var tens = quantity % 100 / 10;
      if (tens != 1) // 1-9, 20-99
      {
        if (SelectedLanguage.GrammaticalNumberModel == GrammaticalNumberModel.Ukrainian)
        {
          var unity = quantity % 10;
          if (unity == 1) // 1, 21, 31, 41, ..., 91
          {
            suffix = SingularSuffix;
          }
          else if (unity > 1 && unity < 5) // 2, 3, 4, 22, 23, 24, ..., 92, 93, 94
          {
            suffix = PaucalSuffix;
          }
        }
        else if (quantity == 1) // 1
        {
          suffix = SingularSuffix;
        }
      }

      return GetString(key + suffix, _settingsService.Language) ?? this[key];
    }
  }


  public ImmutableArray<Language> SupportedLanguages { get; } = new[]
  {
    new Language(new("en-US"), GrammaticalNumberModel.English, SvgIconType.USA),
    new Language(new("uk-UA"), GrammaticalNumberModel.Ukrainian, SvgIconType.Ukraine)
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


internal enum GrammaticalNumberModel
{
  English, Ukrainian
}
