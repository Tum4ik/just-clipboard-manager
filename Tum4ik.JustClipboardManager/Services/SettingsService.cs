using System.Globalization;
using Prism.Events;
using Tum4ik.JustClipboardManager.Events;
using Tum4ik.JustClipboardManager.Properties;

namespace Tum4ik.JustClipboardManager.Services;

internal class SettingsService : ISettingsService
{
  private readonly IEventAggregator _eventAggregator;

  public SettingsService(IEventAggregator eventAggregator)
  {
    _eventAggregator = eventAggregator;
  }


  private CultureInfo? _language;
  public CultureInfo Language
  {
    get => _language ??= CultureInfo.GetCultureInfo(SettingsInterface.Default.Language);
    set
    {
      SettingsInterface.Default.Language = value.Name;
      SettingsInterface.Default.Save();
      _language = value;
      _eventAggregator.GetEvent<LanguageChangedEvent>().Publish();
    }
  }


  private Theme? _theme;
  public Theme Theme
  {
    get => _theme ??= Enum.Parse<Theme>(SettingsInterface.Default.Theme);
    set
    {
      SettingsInterface.Default.Theme = value.ToString();
      SettingsInterface.Default.Save();
      _theme = value;
    }
  }
}
