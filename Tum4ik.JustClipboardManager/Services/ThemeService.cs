using System.Reflection;
using System.Windows;

namespace Tum4ik.JustClipboardManager.Services;

internal class ThemeService : IThemeService
{
  private readonly ResourceDictionary _themeDictionary = new();

  public ThemeService()
  {
    Application.Current.Resources.MergedDictionaries.Add(_themeDictionary);
    SetTheme(CurrentTheme);
  }


  // TODO: take from SettingsService
  public Theme CurrentTheme { get; }


  public void SetTheme(Theme theme)
  {
    _themeDictionary.Source = GetSourceForTheme(theme);
  }


  private Uri? _lightThemeSourced;
  private Uri LightThemeSource => _lightThemeSourced
    ??= new($"pack://application:,,,/{Assembly.GetExecutingAssembly()};component/Themes/LightTheme.xaml");

  private Uri? _darkThemeSource;
  private Uri DarkThemeSource => _darkThemeSource
    ??= new($"pack://application:,,,/{Assembly.GetExecutingAssembly()};component/Themes/DarkTheme.xaml");


  private Uri GetSourceForTheme(Theme theme)
  {
    return theme switch
    {
      Theme.Light => LightThemeSource,
      Theme.Dark => DarkThemeSource,
      _ => throw new NotSupportedException("Unsupported theme."),
    };
  }
}
