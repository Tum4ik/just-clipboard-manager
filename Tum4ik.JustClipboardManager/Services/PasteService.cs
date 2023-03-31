using System.Runtime.InteropServices;
using System.Windows.Input;
using Castle.Core.Internal;
using Tum4ik.JustClipboardManager.Data.Models;

namespace Tum4ik.JustClipboardManager.Services;
internal class PasteService : IPasteService
{
  private readonly IClipboardService _clipboardService;

  public PasteService(IClipboardService clipboardService)
  {
    _clipboardService = clipboardService;
  }


  public void PasteData(nint targetWindowPtr, ICollection<FormattedDataObject> data)
  {
    if (data.Count == 0)
    {
      return;
    }

    _clipboardService.Paste(data);
    SetForegroundWindow(targetWindowPtr);
    SetFocus(targetWindowPtr);

    var ctrl = KeyInterop.VirtualKeyFromKey(Key.LeftCtrl);
    var v = KeyInterop.VirtualKeyFromKey(Key.V);
    keybd_event(ctrl, 0, KEYEVENTF_EXTENDEDKEY, 0);
    keybd_event(v, 0, KEYEVENTF_EXTENDEDKEY, 0);
    keybd_event(v, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
    keybd_event(ctrl, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
  }


  [DllImport("user32.dll")]
  private static extern bool SetForegroundWindow(nint hWnd);

  [DllImport("user32.dll")]
  private static extern int SetFocus(nint hWnd);


  private const int KEYEVENTF_EXTENDEDKEY = 0x0001;
  private const int KEYEVENTF_KEYUP = 0x0002;

  [DllImport("user32.dll")]
  private static extern void keybd_event(int bVk, int bScan, int dwFlags, int dwExtraInfo);
}
