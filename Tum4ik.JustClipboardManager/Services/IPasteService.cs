using System;

namespace Tum4ik.JustClipboardManager.Services;
internal interface IPasteService
{
  void PasteData(IntPtr targetWindowPtr, string? data);
}
