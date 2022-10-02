using System;

namespace Tum4ik.JustClipboardManager.Services;
internal interface IPasteWindowService
{
  public IntPtr WindowHandle { get; }
  void ShowWindow(IntPtr targetWindowToPasteHandle);
  void HideWindow();
}
