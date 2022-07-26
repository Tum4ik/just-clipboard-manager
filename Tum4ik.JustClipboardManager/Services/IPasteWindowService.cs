using System;
using System.Threading.Tasks;

namespace Tum4ik.JustClipboardManager.Services;
internal interface IPasteWindowService
{
  IntPtr GetWindowHandle();
  Task<PasteWindowResult?> ShowWindowAsync(IntPtr targetWindowToPaste);
}


internal record PasteWindowResult(string Data);
