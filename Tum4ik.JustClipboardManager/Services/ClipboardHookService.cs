using System;
using System.Runtime.InteropServices;
using PubSub;
using Tum4ik.JustClipboardManager.EventPayloads;

namespace Tum4ik.JustClipboardManager.Services;
internal sealed class ClipboardHookService : IClipboardHookService, IDisposable
{
  private readonly IPublisher _publisher;

  public ClipboardHookService(IPasteWindowService pasteWindowService,
                              IPublisher publisher)
  {
    _publisher = publisher;

    _windowHandle = pasteWindowService.WindowHandle;
    _nextClipboardViewerHandle = SetClipboardViewer(_windowHandle);
  }


  private readonly IntPtr _windowHandle;
  private IntPtr _nextClipboardViewerHandle;


  public IntPtr HwndHook(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
  {
    switch (msg)
    {
      case 0x0308: // WM_DRAWCLIPBOARD
        OnClipboardChanged();
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
    _publisher.Publish(new ClipboardChanged()); // TODO: use .PublishAsync
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
