using System;
using System.IO;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using Windows.Graphics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace JustClipboardManager;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : Window
{
  public MainWindow()
  {
    InitializeComponent();
    ConfigureWindow();

    WebView.CoreWebView2Initialized += OnCoreWebView2Initialized;
    _ = WebView.EnsureCoreWebView2Async();
  }


  private void ConfigureWindow()
  {
    AppWindow.TitleBar.ExtendsContentIntoTitleBar = true;
    AppWindow.Resize(new SizeInt32(1000, 700));
  }


  private void OnCoreWebView2Initialized(object sender, CoreWebView2InitializedEventArgs e)
  {
    MapVirtualHostNameToUiFolder();
  }


  private void MapVirtualHostNameToUiFolder()
  {
#if DEBUG
    WebView.CoreWebView2.Navigate("http://localhost:4200/#/main-window");
#else
    var uiFolder = Path.Join(AppContext.BaseDirectory, "Resources", "UI");
    WebView.CoreWebView2.SetVirtualHostNameToFolderMapping(
      "angularapp",
      uiFolder,
      CoreWebView2HostResourceAccessKind.Allow
    );
    WebView.CoreWebView2.Navigate("http://angularapp/index.html#/main-window");
#endif
  }
}
