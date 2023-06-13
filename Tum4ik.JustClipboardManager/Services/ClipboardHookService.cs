using Prism.Events;
using Tum4ik.JustClipboardManager.Events;
using Tum4ik.JustClipboardManager.Services.PInvoke;

namespace Tum4ik.JustClipboardManager.Services;
internal sealed class ClipboardHookService : IClipboardHookService, IDisposable
{
  private readonly IEventAggregator _eventAggregator;
  private readonly IUser32DllService _user32Dll;

  public ClipboardHookService(IPasteWindowService pasteWindowService,
                              IEventAggregator eventAggregator,
                              IUser32DllService user32Dll)
  {
    _eventAggregator = eventAggregator;
    _user32Dll = user32Dll;

    _windowHandle = pasteWindowService.WindowHandle;
    _nextClipboardViewerHandle = user32Dll.SetClipboardViewer(_windowHandle);

    _timer.Elapsed += (s, e) => OnClipboardChanged();
  }


  private static readonly object _locker = new();
  private readonly nint _windowHandle;
  private nint _nextClipboardViewerHandle;

  private readonly System.Timers.Timer _timer = new System.Timers.Timer(500)
  {
    AutoReset = false,
    Enabled = false
  };


  public nint HwndHook(nint hWnd, int msg, nint wParam, nint lParam, ref bool handled)
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
        _user32Dll.SendMessage(_nextClipboardViewerHandle, msg, wParam, lParam);
        break;
      case 0x030D: // WM_CHANGECBCHAIN
        if (wParam == _nextClipboardViewerHandle)
        {
          _nextClipboardViewerHandle = lParam;
        }
        else
        {
          _user32Dll.SendMessage(_nextClipboardViewerHandle, msg, wParam, lParam);
        }
        break;
    }
    return nint.Zero;
  }


  private void OnClipboardChanged()
  {
    _eventAggregator.GetEvent<ClipboardChangedEvent>().Publish();
    _timer.Enabled = false;
  }


  public void Dispose()
  {
    _timer.Close();
    _user32Dll.ChangeClipboardChain(_windowHandle, _nextClipboardViewerHandle);
  }
}
