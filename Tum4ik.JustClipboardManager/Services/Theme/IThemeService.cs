namespace Tum4ik.JustClipboardManager.Services.Theme;

internal interface IThemeService
{
  IEnumerable<ColorTheme> Themes { get; }
  ColorTheme SelectedTheme { get; set; }
}
