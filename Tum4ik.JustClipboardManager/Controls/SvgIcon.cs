using Tum4ik.JustClipboardManager.PluginDevKit.Icons;
using Tum4ik.JustClipboardManager.Resources.Icons;

namespace Tum4ik.JustClipboardManager.Controls;
public class SvgIcon : SvgIcon<SvgIconType>
{
  protected override string IconsFolder { get; } = "Resources/Icons";
}
