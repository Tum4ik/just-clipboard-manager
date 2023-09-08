namespace Tum4ik.JustClipboardManager.PluginDevKit.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public sealed class SvgIconResourceAttribute : Attribute
{
  public string SvgIconResourceName { get; }

  public SvgIconResourceAttribute(string svgIconResourceName)
  {
    SvgIconResourceName = svgIconResourceName;
  }
}
