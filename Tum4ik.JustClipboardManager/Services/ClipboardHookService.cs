using Prism.Events;
using Tum4ik.JustClipboardManager.Events;
using Tum4ik.JustClipboardManager.Services.PInvokeWrappers;
using static Windows.Win32.PInvoke;

namespace Tum4ik.JustClipboardManager.Services;
internal sealed class ClipboardHookService : IClipboardHookService, IDisposable
{
  private readonly IEventAggregator _eventAggregator;
  private readonly IUser32DllService _user32Dll;
  private readonly Lazy<IHub> _sentryHub;

  public ClipboardHookService(IPasteWindowService pasteWindowService,
                              IEventAggregator eventAggregator,
                              IUser32DllService user32Dll,
                              Lazy<IHub> sentryHub)
  {
    _eventAggregator = eventAggregator;
    _user32Dll = user32Dll;
    _sentryHub = sentryHub;

    _pasteWindowHandle = pasteWindowService.WindowHandle;
    var isClipboardListenerAdded = user32Dll.AddClipboardFormatListener(_pasteWindowHandle);
    if (!isClipboardListenerAdded)
    {
      _sentryHub.Value.CaptureMessage("AddClipboardFormatListener operation failed", SentryLevel.Fatal);
      // todo: show message to user and close app
    }

    _timer.Elapsed += (s, e) => OnClipboardChanged();
  }


  private static readonly object s_locker = new();
  private readonly nint _pasteWindowHandle;

  private readonly System.Timers.Timer _timer = new(500)
  {
    AutoReset = false,
    Enabled = false
  };


  public nint HwndHook(nint hWnd, int msg, nint wParam, nint lParam, ref bool handled)
  {
    switch ((uint) msg)
    {
      case WM_CLIPBOARDUPDATE:
        // Very often the event raises several times in a raw, however it is the same clipboard change.
        // To prevent that multiple raising of the same event the timer is used.
        lock (s_locker)
        {
          if (!_timer.Enabled)
          {
            _timer.Enabled = true;
          }
        }
        break;
      case WM_CLOSE:
        var isClipboardListenerRemoved = _user32Dll.RemoveClipboardFormatListener(_pasteWindowHandle);
        if (!isClipboardListenerRemoved)
        {
          _sentryHub.Value.CaptureMessage("RemoveClipboardFormatListener operation failed", SentryLevel.Error);
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
  }
}
