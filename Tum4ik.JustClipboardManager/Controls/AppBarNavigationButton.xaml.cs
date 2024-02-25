using System.Windows;
using Tum4ik.JustClipboardManager.Resources.Icons;

namespace Tum4ik.JustClipboardManager.Controls;

/// <summary>
/// Interaction logic for AppBarNavigationButton.xaml
/// </summary>
public partial class AppBarNavigationButton
{
  public AppBarNavigationButton()
  {
    InitializeComponent();
  }


  public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
    nameof(Icon), typeof(SvgIconType?), typeof(AppBarNavigationButton)
  );
  public SvgIconType? Icon
  {
    get => (SvgIconType?) GetValue(IconProperty);
    set => SetValue(IconProperty, value);
  }


  public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
    nameof(Text), typeof(string), typeof(AppBarNavigationButton)
  );
  public string Text
  {
    get => (string) GetValue(TextProperty);
    set => SetValue(TextProperty, value);
  }


  private void RadioButton_Loaded(object sender, RoutedEventArgs e)
  {
    if (IsChecked is true && Command.CanExecute(CommandParameter))
    {
      Command.Execute(CommandParameter);
    }
  }
}
