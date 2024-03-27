using System.Windows.Controls;
using Tum4ik.JustClipboardManager.Resources.Icons;

namespace Tum4ik.JustClipboardManager.Controls.SettingCard;

[DependencyProperty<SvgIconType?>("Icon")]
[DependencyProperty<string>("Header")]
[DependencyProperty<string>("Description")]
internal sealed partial class SettingCard : ContentControl
{
}
