using System.Windows;

namespace Tum4ik.JustClipboardManager.Controls;
/// <summary>
/// Interaction logic for WindowTitleBar.xaml
/// </summary>
public partial class WindowTitleBar
{
  public WindowTitleBar()
  {
    InitializeComponent();
  }


  public static readonly DependencyProperty WindowStateProperty = DependencyProperty.Register(
    nameof(WindowState), typeof(WindowState), typeof(WindowTitleBar)
  );
  public WindowState WindowState
  {
    get => (WindowState) GetValue(WindowStateProperty);
    set => SetValue(WindowStateProperty, value);
  }


  public static readonly DependencyProperty IsWindowActiveProperty = DependencyProperty.Register(
    nameof(IsWindowActive), typeof(bool), typeof(WindowTitleBar)
  );
  public bool IsWindowActive
  {
    get => (bool) GetValue(IsWindowActiveProperty);
    set => SetValue(IsWindowActiveProperty, value);
  }


  private Window _window = null!;


  public override void OnApplyTemplate()
  {
    base.OnApplyTemplate();
    _window = Window.GetWindow(this);
    _window.StateChanged += (s, e) => WindowState = _window.WindowState;
    _window.Activated += (s, e) => IsWindowActive = true;
    _window.Deactivated += (s, e) => IsWindowActive = false;
  }


  private void MinimizeButton_Click(object sender, RoutedEventArgs e)
  {
    SystemCommands.MinimizeWindow(_window);
  }


  private void MaximizeRestoreButton_Click(object sender, RoutedEventArgs e)
  {
    if (_window.WindowState == WindowState.Normal)
    {
      SystemCommands.MaximizeWindow(_window);
    }
    else if (_window.WindowState == WindowState.Maximized)
    {
      SystemCommands.RestoreWindow(_window);
    }
  }


  private void CloseButton_Click(object sender, RoutedEventArgs e)
  {
    _window.Close();
  }
}
