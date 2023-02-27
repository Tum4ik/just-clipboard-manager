using System.Resources;
using Tum4ik.JustClipboardManager.Icons;

namespace Tum4ik.JustClipboardManager.Services.Translation;

internal class TranslationService : ResourceManager, ITranslationService
{
  private readonly ISettingsService _settingsService;

  public TranslationService(ISettingsService settingsService,
                            Type resourceSource)
    : base(resourceSource)
  {
    _settingsService = settingsService;
  }


  public string this[string key]
  {
    get
    {
      return GetString(key, _settingsService.Language) ?? key;
    }
  }


  public Language[] SupportedLanguages { get; } = new[]
  {
    new Language(new("en-US"), SvgIconType.USA),
    new Language(new("uk-UA"), SvgIconType.Ukraine)
  };
}
