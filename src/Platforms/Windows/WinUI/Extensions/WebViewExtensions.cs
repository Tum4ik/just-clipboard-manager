using System;
using System.IO;
using System.Threading.Tasks;
using JustClipboardManager.Application.Services;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;

namespace JustClipboardManager.Extensions;

internal static class WebViewExtensions
{
  public static async Task MapAngularUiAsync(this WebView2 webView, ICommandsService commandsService, string windowUiRoute)
  {
    await webView.EnsureCoreWebView2Async();
    MapVirtualHostNameToUiFolder(webView, windowUiRoute);
    SetupWebMessagesHandler(webView, commandsService);
  }


  private static void MapVirtualHostNameToUiFolder(WebView2 webView, string windowUiRoute)
  {
    var uiFolder = Path.Join(AppContext.BaseDirectory, "Resources", "UI");
    webView.CoreWebView2.SetVirtualHostNameToFolderMapping(
      "angularapp",
      uiFolder,
      CoreWebView2HostResourceAccessKind.Allow
    );
#if DEBUG
    webView.CoreWebView2.Navigate($"http://localhost:4200/#{windowUiRoute}");
#else
    webView.CoreWebView2.Navigate($"http://angularapp/index.html#{windowUiRoute}");
#endif
  }


  private static void SetupWebMessagesHandler(WebView2 webView, ICommandsService commandsService)
  {
    webView.CoreWebView2.WebMessageReceived += async (sender, args) =>
    {
      var message = args.TryGetWebMessageAsString();
      var result = await commandsService.ExecuteCommandAsync(message);
      webView.CoreWebView2.PostWebMessageAsString(result);
    };
  }
}
