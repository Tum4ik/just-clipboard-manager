using System.ComponentModel;
using Microsoft.AppCenter.Crashes;
using Prism.Events;
using Tum4ik.JustClipboardManager.Events;
using Tum4ik.JustClipboardManager.Services.PInvokeWrappers;
using static Windows.Win32.PInvoke;

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
    var isClipboardListenerAdded = user32Dll.AddClipboardFormatListener(_windowHandle);
    if (!isClipboardListenerAdded)
    {
      var ex = new Win32Exception();
      Crashes.TrackError(ex, new Dictionary<string, string>
      {
        { "Description", "AddClipboardFormatListener operation failed" }
      });
      throw ex;
    }

    _timer.Elapsed += (s, e) => OnClipboardChanged();
  }


  private static readonly object s_locker = new();
  private readonly nint _windowHandle;

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
      case WM_DESTROY:
        var isClipboardListenerRemoved = _user32Dll.RemoveClipboardFormatListener(_windowHandle);
        if (!isClipboardListenerRemoved)
        {
          Crashes.TrackError(new Win32Exception(), new Dictionary<string, string>
          {
            { "Description", "RemoveClipboardFormatListener operation failed" }
          });
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
