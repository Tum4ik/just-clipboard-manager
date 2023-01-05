using System;

namespace Tum4ik.JustClipboardManager.Services;
internal interface IPasteWindowService
{
  nint WindowHandle { get; }
  void ShowWindow(nint targetWindowToPasteHandle);
  void HideWindow();
}
