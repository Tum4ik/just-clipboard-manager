using System.Collections.Immutable;

namespace Tum4ik.JustClipboardManager.Services.Theme;

internal interface IThemeService
{
  ImmutableArray<ColorTheme> Themes { get; }
  ColorTheme SelectedTheme { get; set; }
}
