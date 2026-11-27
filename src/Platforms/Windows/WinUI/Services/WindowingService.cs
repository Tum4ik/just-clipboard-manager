using System;
using System.Collections.Immutable;
using System.IO;
using System.Threading.Tasks;
using JustClipboardManager.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;

namespace JustClipboardManager.Services;

internal class WindowingService : IWindowingService
{
  private readonly DispatcherQueue _dispatcherQueue;

  /// <summary>
  /// At least one window must always be created for the application to function normally.
  /// </summary>
  private readonly PasteWindow _pasteWindow;
  private readonly IServiceProvider _serviceProvider;
  private readonly ICommandsService _commandsService;

  public WindowingService(PasteWindow pasteWindow,
                          IServiceProvider serviceProvider,
                          ICommandsService commandsService)
  {
    _pasteWindow = pasteWindow;
    _serviceProvider = serviceProvider;
    _commandsService = commandsService;
    _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
  }


  public void ShowPasteWindow()
  {
    ShowOnMainThread(ShowPasteWindowInternal);
  }


  public void ShowMainWindow()
  {
    ShowOnMainThread(ShowMainWindowInternal);
  }


  public async Task PublishEventForMainWindowAsync(string appEventJson)
  {
    if (_mainWindow is null)
    {
      return;
    }

    await PostEventAsync(_mainWindow, appEventJson);
  }


  private void ShowPasteWindowInternal()
  {
    _pasteWindow.AppWindow.Show();
  }


  private MainWindow? _mainWindow;

  private async void ShowMainWindowInternal()
  {
    _mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
    await MapAngularUiAsync(_mainWindow.WebView, "main-window");
    _mainWindow.Closed += OnMainWindowClosed;
    _mainWindow.Activate();
  }


  private async void OnMainWindowClosed(object sender, WindowEventArgs args)
  {
    _mainWindow?.Closed -= OnMainWindowClosed;
    _mainWindow?.WebView.CoreWebView2.WebMessageReceived -= OnCommandReceived;
    _mainWindow = null;
  }


  private void ShowOnMainThread(Action action)
  {
    if (_dispatcherQueue != null && !_dispatcherQueue.HasThreadAccess)
    {
      _dispatcherQueue.TryEnqueue(new(action));
      return;
    }
    action();
  }


  private static async Task PostEventAsync<TWindow>(TWindow window, string appEventJson)
    where TWindow : Window, IAppWindow
  {
    var webView = window.WebView;
    await webView.EnsureCoreWebView2Async();
    webView.CoreWebView2.PostWebMessageAsJson(appEventJson);
  }


  public async Task MapAngularUiAsync(WebView2 webView, string windowUiRoute)
  {
    await webView.EnsureCoreWebView2Async();
    MapVirtualHostNameToUiFolder(webView, windowUiRoute);
    webView.CoreWebView2.WebMessageReceived += OnCommandReceived;
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


  private async void OnCommandReceived(CoreWebView2 sender, CoreWebView2WebMessageReceivedEventArgs args)
  {
    var commandResultJson = await _commandsService.ExecuteCommandAsync(args.WebMessageAsJson);
    sender.PostWebMessageAsJson(commandResultJson);
  }
}
