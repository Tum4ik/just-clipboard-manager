using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace JustClipboardManager
{
  /// <summary>
  /// Provides application-specific behavior to supplement the default Application class.
  /// </summary>
  public partial class App : Application
  {
    private Window? _window;

    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Invoked when the application is launched.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
      _window = new MainWindow();
      // _window.AppWindow.TitleBar.ExtendsContentIntoTitleBar = true;
      if (_window.AppWindow.Presenter is OverlappedPresenter presenter)
      {
        // presenter.IsMaximizable = false;
        // presenter.IsMinimizable = false;
        presenter.SetBorderAndTitleBar(true, false);
      }
      _window.Activate();
    }
  }
}
