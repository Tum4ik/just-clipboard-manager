using System.Windows;
using System.Windows.Input;
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


  private ICommand? _command;


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


  protected override void OnChecked(RoutedEventArgs e)
  {
    base.OnChecked(e);
    if (_command is not null && _command.CanExecute(CommandParameter))
    {
      _command.Execute(CommandParameter);
    }
  }


  private void RadioButton_Loaded(object sender, RoutedEventArgs e)
  {
    _command = Command;
    Command = null;
    if (IsChecked is true && _command is not null && _command.CanExecute(CommandParameter))
    {
      _command.Execute(CommandParameter);
    }
  }
}
