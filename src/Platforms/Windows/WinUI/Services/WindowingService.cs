using JustClipboardManager.Application.Services;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Windowing;

namespace JustClipboardManager.Services;

internal class WindowingService : IWindowingService
{
  private readonly DispatcherQueue _dispatcherQueue;
  private readonly PasteWindow _pasteWindow = new();

  public WindowingService()
  {
    _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
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
    new MainWindow().Activate();
  }
}
