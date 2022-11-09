using System.Runtime.InteropServices;
using Prism.Events;
using Tum4ik.JustClipboardManager.Events;

namespace Tum4ik.JustClipboardManager.Services;
internal sealed class ClipboardHookService : IClipboardHookService, IDisposable
{
  private readonly IEventAggregator _eventAggregator;

  public ClipboardHookService(IPasteWindowService pasteWindowService,
                              IEventAggregator eventAggregator)
  {
    _eventAggregator = eventAggregator;

    _windowHandle = pasteWindowService.WindowHandle;
    _nextClipboardViewerHandle = SetClipboardViewer(_windowHandle);

    _timer.Elapsed += (s, e) => OnClipboardChanged();
  }


  private static readonly object _locker = new();
  private readonly IntPtr _windowHandle;
  private IntPtr _nextClipboardViewerHandle;

  private readonly System.Timers.Timer _timer = new System.Timers.Timer(500)
  {
    AutoReset = false,
    Enabled = false
  };


  public IntPtr HwndHook(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
  {
    switch (msg)
    {
      case 0x0308: // WM_DRAWCLIPBOARD
        // Very often the event raises several times in a raw, however it is the same clipboard change.
        // To prevent that multiple raising of the same event the timer is used.
        lock (_locker)
        {
          if (!_timer.Enabled)
          {
            _timer.Enabled = true;
          }
        }
        SendMessage(_nextClipboardViewerHandle, msg, wParam, lParam);
        break;
      case 0x030D: // WM_CHANGECBCHAIN
        if (wParam == _nextClipboardViewerHandle)
        {
          _nextClipboardViewerHandle = lParam;
        }
        else
        {
          SendMessage(_nextClipboardViewerHandle, msg, wParam, lParam);
        }
        break;
    }
    return IntPtr.Zero;
  }


  private void OnClipboardChanged()
  {
    _eventAggregator.GetEvent<ClipboardChangedEvent>().Publish();
    _timer.Enabled = false;
  }


  public void Dispose()
  {
    ChangeClipboardChain(_windowHandle, _nextClipboardViewerHandle);
  }


  [DllImport("user32.dll")]
  private static extern IntPtr SetClipboardViewer(IntPtr hWndNewViewer);

  [DllImport("user32.dll")]
  private static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);

  [DllImport("user32.dll")]
  private static extern int SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
}
