using System.Resources;

namespace Tum4ik.JustClipboardManager.PluginDevKit.Services;
internal sealed class PluginTranslationService : IPluginTranslationService
{
  private readonly ResourceManager _resourceManager;
  private readonly IPluginSettingsService _settingsService;

  public PluginTranslationService(ResourceManager resourceManager,
                                  IPluginSettingsService settingsService)
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
