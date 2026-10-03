using JustClipboardManager.Application.Services;
using JustClipboardManager.Extensions;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Windows.Graphics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace JustClipboardManager;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : Window
{
  public MainWindow(ICommandsService commandsService)
  {
    InitializeComponent();
    ConfigureWindow();

    _ = WebView.MapAngularUiAsync(commandsService, "main-window");
  }


  private void ConfigureWindow()
  {
    AppWindow.SetIcon("Resources/UI/assets/shared/icon.ico");
    AppWindow.Resize(new SizeInt32(1000, 700));
    var titleBar = AppWindow.TitleBar;
    titleBar.ExtendsContentIntoTitleBar = true;
    titleBar.ButtonBackgroundColor = Colors.Transparent;
  }
}
