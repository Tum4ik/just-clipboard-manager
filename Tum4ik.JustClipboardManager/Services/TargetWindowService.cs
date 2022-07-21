using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

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
}
