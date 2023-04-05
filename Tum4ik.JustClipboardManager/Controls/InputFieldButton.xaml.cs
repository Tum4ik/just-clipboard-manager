using System.Windows;
using Tum4ik.JustClipboardManager.Icons;

namespace Tum4ik.JustClipboardManager.Controls;
/// <summary>
/// Interaction logic for InputFieldButton.xaml
/// </summary>
public partial class InputFieldButton
{
  public InputFieldButton()
  {
    InitializeComponent();
  }


  public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
    nameof(Icon), typeof(SvgIconType?), typeof(InputFieldButton)
  );
  public SvgIconType? Icon
  {
    get => (SvgIconType?) GetValue(IconProperty);
    set => SetValue(IconProperty, value);
  }
}
