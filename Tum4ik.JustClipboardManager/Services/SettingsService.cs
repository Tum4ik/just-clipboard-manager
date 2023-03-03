using System.Globalization;
using Tum4ik.JustClipboardManager.Properties;

namespace Tum4ik.JustClipboardManager.Services;

internal class SettingsService : ISettingsService
{
  private CultureInfo? _language;
  public CultureInfo Language
  {
    get => _language ??= CultureInfo.GetCultureInfo(SettingsInterface.Default.Language);
    set
    {
      SettingsInterface.Default.Language = value.Name;
      SettingsInterface.Default.Save();
      _language = value;
    }
  }


  private string? _theme;
  public string Theme
  {
    get => _theme ??= SettingsInterface.Default.Theme;
    set
    {
      SettingsInterface.Default.Theme = value;
      SettingsInterface.Default.Save();
      _theme = value;
    }
  }
}
