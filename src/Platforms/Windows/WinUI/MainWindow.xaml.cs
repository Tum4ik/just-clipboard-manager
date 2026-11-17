using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Google.Protobuf;
using JustClipboardManager.Application.Services;
using JustClipboardManager.Extensions;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.Web.WebView2.Core;
using Windows.Graphics;
using JustClipboardManager.Application.IPC.Events;

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


  public async Task PostEventAsync(AppEvent appEvent)
  {
    await WebView.EnsureCoreWebView2Async();
    // todo: extract logic to use same options for all windows
    var payload = appEvent.ToByteArray();
    using var sharedBuffer = WebView.CoreWebView2.Environment.CreateSharedBuffer((ulong)payload.Length);
    using var stream = sharedBuffer.OpenStream();
    await stream.WriteAsync(payload.AsBuffer());

    WebView.CoreWebView2.PostSharedBufferToScript(
      sharedBuffer,
      CoreWebView2SharedBufferAccess.ReadOnly,
      string.Empty
    );
  }


  private void ConfigureWindow()
  {
    AppWindow.SetIcon("Resources/UI/assets/shared/icon.ico");
    AppWindow.Resize(new SizeInt32(1000, 700));
    var titleBar = AppWindow.TitleBar;
    titleBar.ExtendsContentIntoTitleBar = true;
    titleBar.ButtonBackgroundColor = Colors.Transparent;
    titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
  }
}
