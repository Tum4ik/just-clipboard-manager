using System.Resources;

namespace Tum4ik.JustClipboardManager.PluginDevKit.Services;
internal class PluginTranslationService : IPluginTranslationService
{
  private readonly ResourceManager _resourceManager;
  private readonly ISettingsService _settingsService;

  public PluginTranslationService(ResourceManager resourceManager,
                                  ISettingsService settingsService)
  {
    _resourceManager = resourceManager;
    _settingsService = settingsService;
  }


  public string this[string key]
  {
    get
    {
      return _resourceManager.GetString(key, _settingsService.Language) ?? key;
    }
  }
}
