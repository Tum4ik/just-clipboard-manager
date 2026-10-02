using System;
using JustClipboardManager.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Dispatching;

namespace JustClipboardManager.Services;

internal class WindowingService : IWindowingService
{
  private readonly DispatcherQueue _dispatcherQueue;

  /// <summary>
  /// At least one window must always be created for the application to function normally.
  /// </summary>
  private readonly PasteWindow _pasteWindow = new();
  private readonly IServiceProvider _serviceProvider;

  public WindowingService(IServiceProvider serviceProvider)
  {
    _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    _serviceProvider = serviceProvider;
  }


  public void ShowPasteWindow()
  {
    _pasteWindow.AppWindow.Show();
  }


  public void ShowMainWindow()
  {
    if (_dispatcherQueue != null && !_dispatcherQueue.HasThreadAccess)
    {
      _dispatcherQueue.TryEnqueue(ShowMainWindowInternal);
      return;
    }

    ShowMainWindowInternal();
  }


  private void ShowMainWindowInternal()
  {
    var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
    mainWindow.Activate();
  }
}
