using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Input;

namespace Tum4ik.JustClipboardManager.Services;
internal class TargetWindowService : ITargetWindowService
{
  public void PasteData(string data)
  {
    var targetWindowPtr = GetTargetWindowPtr();
    if (targetWindowPtr == IntPtr.Zero)
    {
      return;
    }

    SetForegroundWindow(targetWindowPtr);
    SetFocus(targetWindowPtr);

    var ctrl = KeyInterop.VirtualKeyFromKey(Key.LeftCtrl);
    var v = KeyInterop.VirtualKeyFromKey(Key.V);
    keybd_event(ctrl, 0, KEYEVENTF_EXTENDEDKEY, 0);
    keybd_event(v, 0, KEYEVENTF_EXTENDEDKEY, 0);
    keybd_event(v, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
    keybd_event(ctrl, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
  }


  private IntPtr GetTargetWindowPtr()
  {
    var targetWindowPtrs = new List<IntPtr>();
    EnumWindows((hWnd, lParam) => 
    {
      var winTitle = new StringBuilder(256);
      GetWindowText(hWnd, winTitle, 256);
      if (IsWindowVisible(hWnd) && winTitle.Length > 0)
      {
        targetWindowPtrs.Add(hWnd);
      }
      return true;
    }, IntPtr.Zero);
    // Try to return the second window in the list, because the first window is our Paste window
    if (targetWindowPtrs.Count >= 2)
    {
      return targetWindowPtrs[1];
    }
    return IntPtr.Zero;
  }


  private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

  [DllImport("user32.dll")]
  private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

  [DllImport("user32.dll")]
  private static extern bool IsWindowVisible(IntPtr hWnd);

  [DllImport("user32.dll")]
  private static extern int GetWindowText(IntPtr hWnd, StringBuilder title, int size);

  [DllImport("user32.dll")]
  private static extern bool SetForegroundWindow(IntPtr hWnd);

  [DllImport("user32.dll")]
  private static extern int SetFocus(IntPtr hWnd);


  private const int KEYEVENTF_EXTENDEDKEY = 0x0001;
  private const int KEYEVENTF_KEYUP = 0x0002;

  [DllImport("user32.dll")]
  private static extern void keybd_event(int bVk, int bScan, int dwFlags, int dwExtraInfo);
}
