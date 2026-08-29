using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace JustClipboardManager;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class PasteWindow : Window
{
  public PasteWindow()
  {
    InitializeComponent();

    if (AppWindow.Presenter is OverlappedPresenter presenter)
    {
      presenter.SetBorderAndTitleBar(true, false);
    }
  }
}
