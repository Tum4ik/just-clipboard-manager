using System.Globalization;
using Tum4ik.JustClipboardManager.Icons;

namespace Tum4ik.JustClipboardManager.Services.Translation;

internal class Language
{
  public Language(CultureInfo culture, SvgIconType icon)
  {
    Culture = culture;
    Icon = icon;
  }


  public CultureInfo Culture { get; }
  public SvgIconType Icon { get; }
}
