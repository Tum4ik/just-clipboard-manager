using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Tum4ik.JustClipboardManager.Controls;
/// <summary>
/// Interaction logic for WinUiSlider.xaml
/// </summary>
public partial class WinUiSlider
{
  public WinUiSlider()
  {
    InitializeComponent();
    AddHandler(PreviewMouseLeftButtonDownEvent, new RoutedEventHandler((s, e) =>
    {
      _isClickedInSlider = true;
    }), true);
    AddHandler(PreviewMouseLeftButtonUpEvent, new RoutedEventHandler((s, e) =>
    {
      _isClickedInSlider = false;
    }), true);
  }


  private Thumb? _thumb;
  private bool _isClickedInSlider;


  public override void OnApplyTemplate()
  {
    base.OnApplyTemplate();
    _thumb = (GetTemplateChild("PART_Track") as Track)?.Thumb;
  }


  protected override void OnPreviewMouseMove(MouseEventArgs e)
  {
    if (_isClickedInSlider && e.LeftButton == MouseButtonState.Pressed)
    {
      _thumb?.RaiseEvent(new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, MouseButton.Left)
      {
        RoutedEvent = MouseLeftButtonDownEvent,
        Source = e.Source
      });
    }
  }
}
