using Tum4ik.JustClipboardManager.Resources.Icons;

namespace Tum4ik.JustClipboardManager.Services.Theme;

internal class ColorTheme
{
  public ColorTheme(ThemeType themeType, SvgIconType icon, string xamlFileName)
  {
    ThemeType = themeType;
    Name = themeType.ToString();
    Icon = icon;
    XamlFileName = xamlFileName;
  }

  public ThemeType ThemeType { get; }
  public string Name { get; }
  public SvgIconType Icon { get; }
  public string XamlFileName { get; }
}


internal enum ThemeType
{
  Light, Dark
}
