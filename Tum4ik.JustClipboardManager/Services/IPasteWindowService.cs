using System;

namespace Tum4ik.JustClipboardManager.Services;
internal interface IPasteWindowService
{
  IntPtr GetWindowHandle();
  void ShowWindow();
}
