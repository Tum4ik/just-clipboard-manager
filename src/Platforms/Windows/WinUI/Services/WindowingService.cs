using System;
using System.Threading.Tasks;
using JustClipboardManager.Application.Common.Abstractions;
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
  private readonly PasteWindow _pasteWindow;
  private readonly IServiceProvider _serviceProvider;

  public WindowingService(PasteWindow pasteWindow,
                          IServiceProvider serviceProvider)
  {
    _pasteWindow = pasteWindow;
    _serviceProvider = serviceProvider;
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

  public async Task PublishEventForMainWindowAsync<TEvent>(TEvent appEvent) where TEvent : IAppEvent, new()
  {
    if (_mainWindow is null)
    {
      return;
    }

    await _mainWindow.PostEventAsync(appEvent);
  }


  private void ShowPasteWindowInternal()
  {
    _pasteWindow.AppWindow.Show();
  }


  private MainWindow? _mainWindow;

  private void ShowMainWindowInternal()
  {
    _mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
    _mainWindow.Closed += (sender, args) =>
    {
      _mainWindow = null;
    };
    _mainWindow.Activate();
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
}
