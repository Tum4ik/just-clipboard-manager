using System.Windows;
using System.Windows.Input;
using Tum4ik.JustClipboardManager.Resources.Icons;

namespace Tum4ik.JustClipboardManager.Controls;

/// <summary>
/// Interaction logic for WinUiButton.xaml
/// </summary>
public partial class WinUiButton
{
  public WinUiButton()
  {
    InitializeComponent();
  }


  public static readonly DependencyProperty IsKeyboardNavigatedFocusProperty = DependencyProperty.Register(
    nameof(IsKeyboardNavigatedFocus), typeof(bool), typeof(WinUiButton)
  );
  public bool IsKeyboardNavigatedFocus
  {
    get => (bool) GetValue(IsKeyboardNavigatedFocusProperty);
    set => SetValue(IsKeyboardNavigatedFocusProperty, value);
  }


  public static readonly DependencyProperty IconPositionProperty = DependencyProperty.Register(
    nameof(IconPosition), typeof(ButtonIconPosition), typeof(WinUiButton)
  );
  public ButtonIconPosition IconPosition
  {
    get => (ButtonIconPosition) GetValue(IconPositionProperty);
    set => SetValue(IconPositionProperty, value);
  }


  public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
    nameof(Icon), typeof(SvgIconType?), typeof(WinUiButton)
  );
  public SvgIconType? Icon
  {
    get => (SvgIconType?) GetValue(IconProperty);
    set => SetValue(IconProperty, value);
  }


  public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
    nameof(Text), typeof(string), typeof(WinUiButton)
  );
  public string? Text
  {
    get => (string?) GetValue(TextProperty);
    set => SetValue(TextProperty, value);
  }


  public static readonly DependencyProperty IsAccentProperty = DependencyProperty.Register(
    nameof(IsAccent), typeof(bool), typeof(WinUiButton)
  );
  public bool IsAccent
  {
    get => (bool) GetValue(IsAccentProperty);
    set => SetValue(IsAccentProperty, value);
  }


  protected override void OnGotFocus(RoutedEventArgs e)
  {
    base.OnGotFocus(e);
    IsKeyboardNavigatedFocus = InputManager.Current.MostRecentInputDevice is KeyboardDevice;
  }


  protected override void OnLostFocus(RoutedEventArgs e)
  {
    base.OnLostFocus(e);
    IsKeyboardNavigatedFocus = false;
  }
}


public enum ButtonIconPosition
{
  Before, After
}
