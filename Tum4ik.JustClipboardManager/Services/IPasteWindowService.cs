using System;

namespace Tum4ik.JustClipboardManager.Services;
internal interface IPasteWindowService
{
  IntPtr WindowHandle { get; }
  void ShowWindow(IntPtr targetWindowToPasteHandle);
  void HideWindow();
}
