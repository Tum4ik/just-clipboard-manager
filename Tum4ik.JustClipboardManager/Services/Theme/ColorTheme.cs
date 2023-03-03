using Tum4ik.JustClipboardManager.Icons;

namespace Tum4ik.JustClipboardManager.Services.Theme;

internal class ColorTheme
{
  public ColorTheme(string name, SvgIconType icon, string xamlFileName)
  {
    Name = name;
    Icon = icon;
    XamlFileName = xamlFileName;
  }


  public string Name { get;  }
  public SvgIconType Icon { get; }
  public string XamlFileName { get; set; }
}
