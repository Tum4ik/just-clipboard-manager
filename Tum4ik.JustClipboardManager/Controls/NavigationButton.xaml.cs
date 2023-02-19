using System.Windows;
using Tum4ik.JustClipboardManager.Icons;

namespace Tum4ik.JustClipboardManager.Controls;

/// <summary>
/// Interaction logic for NavigationButton.xaml
/// </summary>
public partial class NavigationButton
{
  public NavigationButton()
  {
    InitializeComponent();
  }


  public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
    nameof(Icon), typeof(SvgIconType?), typeof(NavigationButton)
  );
  public SvgIconType? Icon
  {
    get => (SvgIconType?) GetValue(IconProperty);
    set => SetValue(IconProperty, value);
  }


  public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
    nameof(Text), typeof(string), typeof(NavigationButton)
  );
  public string Text
  {
    get => (string) GetValue(TextProperty);
    set => SetValue(TextProperty, value);
  }
}
