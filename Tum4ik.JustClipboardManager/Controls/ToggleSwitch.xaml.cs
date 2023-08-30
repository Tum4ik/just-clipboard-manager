using System.Windows;
using System.Windows.Input;

namespace Tum4ik.JustClipboardManager.Controls;

/// <summary>
/// Interaction logic for ToggleSwitch.xaml
/// </summary>
public partial class ToggleSwitch
{
  public ToggleSwitch()
  {
    InitializeComponent();
  }


  public static readonly DependencyProperty IsKeyboardNavigatedFocusProperty = DependencyProperty.Register(
    nameof(IsKeyboardNavigatedFocus), typeof(bool), typeof(ToggleSwitch)
  );
  public bool IsKeyboardNavigatedFocus
  {
    get => (bool) GetValue(IsKeyboardNavigatedFocusProperty);
    set => SetValue(IsKeyboardNavigatedFocusProperty, value);
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
