namespace Tum4ik.JustClipboardManager.Icons;

[AttributeUsage(AttributeTargets.Field)]
public class SvgIconResourceAttribute : Attribute
{
  public string SvgIconResourceName { get; }

  public SvgIconResourceAttribute(string svgIconResourceName)
  {
    SvgIconResourceName = svgIconResourceName;
  }
}
