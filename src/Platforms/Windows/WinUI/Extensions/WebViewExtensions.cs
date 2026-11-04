using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using JustClipboardManager.Application.Common;
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


  private static readonly JsonSerializerOptions s_deserializationOptions = new()
  {
    PropertyNameCaseInsensitive = true
  };

  private static readonly JsonSerializerOptions s_serializationOptions = new()
  {
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
  };

  private static void SetupWebMessagesHandler(WebView2 webView, ICommandsService commandsService)
  {
    webView.CoreWebView2.WebMessageReceived += async (sender, args) =>
    {
      var message = args.WebMessageAsJson;
      var commandMessage = JsonSerializer.Deserialize<CommandMessage>(message, s_deserializationOptions);
      ArgumentNullException.ThrowIfNull(commandMessage);
      var result = await commandsService.ExecuteCommandAsync(commandMessage.CommandName, commandMessage.CommandParameter);
      var backendProcessEvent = new BackendProcessEvent
      {
        Id = commandMessage.EventId,
        Name = commandMessage.CommandName,
        Payload = result,
      };
      var commandResultJson = JsonSerializer.Serialize(backendProcessEvent, s_serializationOptions);
      webView.CoreWebView2.PostWebMessageAsJson(commandResultJson);
    };
  }
}
