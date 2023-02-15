using System.Windows;
using Tum4ik.JustClipboardManager.Icons;

namespace Tum4ik.JustClipboardManager.Controls;

/// <summary>
/// Interaction logic for TabButton.xaml
/// </summary>
public partial class TabButton
{
  public TabButton()
  {
    InitializeComponent();
  }


  public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
    nameof(Icon), typeof(SvgIconType?), typeof(TabButton)
  );
  public SvgIconType? Icon
  {
    get => (SvgIconType?) GetValue(IconProperty);
    set => SetValue(IconProperty, value);
  }


  public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
    nameof(Text), typeof(string), typeof(TabButton)
  );
  public string Text
  {
    get => (string) GetValue(TextProperty);
    set => SetValue(TextProperty, value);
  }


  public static readonly DependencyProperty UncheckedLeftSeparatorVisibilityProperty = DependencyProperty.Register(
    nameof(UncheckedLeftSeparatorVisibility), typeof(Visibility), typeof(TabButton)
  );
  public Visibility UncheckedLeftSeparatorVisibility
  {
    get => (Visibility) GetValue(UncheckedLeftSeparatorVisibilityProperty);
    set => SetValue(UncheckedLeftSeparatorVisibilityProperty, value);
  }


  public static readonly DependencyProperty UncheckedRightSeparatorVisibilityProperty = DependencyProperty.Register(
    nameof(UncheckedRightSeparatorVisibility), typeof(Visibility), typeof(TabButton)
  );
  public Visibility UncheckedRightSeparatorVisibility
  {
    get => (Visibility) GetValue(UncheckedRightSeparatorVisibilityProperty);
    set => SetValue(UncheckedRightSeparatorVisibilityProperty, value);
  }


  private void RadioButton_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
  {
    IsPressed = true;
  }

  private void RadioButton_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
  {
    IsPressed = false;
  }

  private void RadioButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
  {
    IsPressed = false;
  }
}
