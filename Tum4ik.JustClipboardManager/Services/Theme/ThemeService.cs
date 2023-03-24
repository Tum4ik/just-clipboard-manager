using System.Reflection;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using Tum4ik.JustClipboardManager.Icons;

namespace Tum4ik.JustClipboardManager.Services.Theme;

internal partial class ThemeService : ObservableObject, IThemeService
{
  private readonly ResourceDictionary _themeDictionary = new();
  private readonly ISettingsService _settingsService;

  public ThemeService(ISettingsService settingsService)
  {
    _settingsService = settingsService;

    Application.Current.Resources.MergedDictionaries.Add(_themeDictionary);
    _selectedTheme = Themes.SingleOrDefault(t => t.Name == settingsService.Theme) ?? Themes.First();
    SetTheme(_selectedTheme);
  }


  public IEnumerable<ColorTheme> Themes { get; } = new[]
  {
    new ColorTheme("Light", SvgIconType.LightMode, "LightTheme.xaml"),
    new ColorTheme("Dark", SvgIconType.DarkMode, "DarkTheme.xaml")
  };


  [ObservableProperty] private ColorTheme _selectedTheme;
  partial void OnSelectedThemeChanged(ColorTheme value)
  {
    SetTheme(value);
  }


  private void SetTheme(ColorTheme theme)
  {
    _themeDictionary.Source = GetSourceForTheme(theme.XamlFileName);
    _settingsService.Theme = theme.Name;
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
