using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Tum4ik.JustClipboardManager.Resources.Icons;

namespace Tum4ik.JustClipboardManager.Controls;

internal class CaptionButton : Button
{
  public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
    nameof(CornerRadius), typeof(CornerRadius), typeof(CaptionButton)
  );
  public CornerRadius CornerRadius
  {
    get => (CornerRadius) GetValue(CornerRadiusProperty);
    set => SetValue(CornerRadiusProperty, value);
  }


  public static readonly DependencyProperty HoverBackgroundBrushProperty = DependencyProperty.Register(
    nameof(HoverBackgroundBrush), typeof(Brush), typeof(CaptionButton)
  );
  public Brush HoverBackgroundBrush
  {
    get => (Brush) GetValue(HoverBackgroundBrushProperty);
    set => SetValue(HoverBackgroundBrushProperty, value);
  }


  public static readonly DependencyProperty PressedBackgroundBrushProperty = DependencyProperty.Register(
    nameof(PressedBackgroundBrush), typeof(Brush), typeof(CaptionButton)
  );
  public Brush PressedBackgroundBrush
  {
    get => (Brush) GetValue(PressedBackgroundBrushProperty);
    set => SetValue(PressedBackgroundBrushProperty, value);
  }


  public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
    nameof(Icon), typeof(SvgIconType), typeof(CaptionButton)
  );
  public SvgIconType Icon
  {
    get => (SvgIconType) GetValue(IconProperty);
    set => SetValue(IconProperty, value);
  }


  public static readonly DependencyProperty IconColorProperty = DependencyProperty.Register(
    nameof(IconColor), typeof(Brush), typeof(CaptionButton)
  );
  public Brush IconColor
  {
    get => (Brush) GetValue(IconColorProperty);
    set => SetValue(IconColorProperty, value);
  }


  public static readonly DependencyProperty HoverIconBrushProperty = DependencyProperty.Register(
    nameof(HoverIconBrush), typeof(Brush), typeof(CaptionButton)
  );
  public Brush HoverIconBrush
  {
    get => (Brush) GetValue(HoverIconBrushProperty);
    set => SetValue(HoverIconBrushProperty, value);
  }


  public static readonly DependencyProperty PressedIconBrushProperty = DependencyProperty.Register(
    nameof(PressedIconBrush), typeof(Brush), typeof(CaptionButton)
  );
  public Brush PressedIconBrush
  {
    get => (Brush) GetValue(PressedIconBrushProperty);
    set => SetValue(PressedIconBrushProperty, value);
  }


  public static readonly DependencyProperty IsKeyboardNavigatedFocusProperty = DependencyProperty.Register(
    nameof(IsKeyboardNavigatedFocus), typeof(bool), typeof(CaptionButton)
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
