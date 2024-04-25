using System.Windows.Media;
using Tum4ik.JustClipboardManager.Resources.Icons;

namespace Tum4ik.JustClipboardManager.Controls;

[DependencyProperty<SvgIconType?>("Icon")]
[DependencyProperty<Brush>("IconColor")]
public partial class InputFieldButton
{
  public InputFieldButton()
  {
    InitializeComponent();
  }
}
