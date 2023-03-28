using System.Windows;
using System.Windows.Input;
using Tum4ik.JustClipboardManager.ViewModels.Main.Settings;

namespace Tum4ik.JustClipboardManager.Views.Main.Settings;
/// <summary>
/// Interaction logic for EditHotkeyDialog.xaml
/// </summary>
public partial class EditHotkeyDialog
{
  public EditHotkeyDialog()
  {
    InitializeComponent();

    Loaded += (s, e) =>
    {
      _window = Window.GetWindow(this);
      _window.PreviewKeyDown += WindowPreviewKeyDown;
      _window.PreviewKeyUp += WindowPreviewKeyUp;
    };
    Unloaded += (s, e) =>
    {
      if (_window is not null)
      {
        _window.PreviewKeyDown -= WindowPreviewKeyDown;
        _window.PreviewKeyUp -= WindowPreviewKeyUp;
      }
    };
  }


  private Window? _window;


  private void WindowPreviewKeyDown(object sender, KeyEventArgs e)
  {
    e.Handled = true;
    if (DataContext is EditHotkeyDialogViewModel viewModel)
    {
      viewModel.KeyboardKeyDownCommand.Execute(e);
    }
  }


  private void WindowPreviewKeyUp(object sender, KeyEventArgs e)
  {
    e.Handled = true;
    if (DataContext is EditHotkeyDialogViewModel viewModel)
    {
      viewModel.KeyboardKeyUpCommand.Execute(e);
    }
  }
}
