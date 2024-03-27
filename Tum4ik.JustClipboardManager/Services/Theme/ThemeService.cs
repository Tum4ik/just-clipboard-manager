using System.Collections.Immutable;
using System.Reflection;
using System.Windows;
using Prism.Events;
using Tum4ik.JustClipboardManager.Events;
using Tum4ik.JustClipboardManager.Resources.Icons;

namespace Tum4ik.JustClipboardManager.Services.Theme;

internal class ThemeService : IThemeService
{
  private readonly ResourceDictionary _themeDictionary = new();
  private readonly ISettingsService _settingsService;
  private readonly IEventAggregator _eventAggregator;

  public ThemeService(ISettingsService settingsService,
                      IEventAggregator eventAggregator,
                      IAppResourcesService appResourcesService)
  {
    _settingsService = settingsService;
    _eventAggregator = eventAggregator;

    appResourcesService.Resources.MergedDictionaries.Add(_themeDictionary);
    SetTheme(SelectedTheme);
  }


  public ImmutableArray<ColorTheme> Themes { get; } =
  [
    new ColorTheme(ThemeType.Light, SvgIconType.LightMode, "LightTheme.xaml"),
    new ColorTheme(ThemeType.Dark, SvgIconType.DarkMode, "DarkTheme.xaml")
  ];


  private ColorTheme? _selectedTheme;
  public ColorTheme SelectedTheme
  {
    get => _selectedTheme ??= Themes.SingleOrDefault(t => t.Name == _settingsService.Theme) ?? Themes.First();
    set
    {
      if (_selectedTheme != value)
      {
        _selectedTheme = value;
        SetTheme(value);
      }
    }
  }


  private void SetTheme(ColorTheme theme)
  {
    _themeDictionary.Source = GetSourceForTheme(theme.XamlFileName);
    _settingsService.Theme = theme.Name;
    _eventAggregator.GetEvent<ThemeChangedEvent>().Publish();
  }


  private readonly Dictionary<string, Uri> _themeNameToSource = new();
  private Uri GetSourceForTheme(string xamlFileName)
  {
    if (_themeNameToSource.TryGetValue(xamlFileName, out var existingSource))
    {
      return existingSource;
    }

    var source = new Uri($"pack://application:,,,/{Assembly.GetExecutingAssembly()};component/Themes/{xamlFileName}");
    _themeNameToSource[xamlFileName] = source;
    return source;
  }
}
