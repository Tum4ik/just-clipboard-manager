using System;

namespace Tum4ik.JustClipboardManager.Services;
internal interface IHookService
{
  IntPtr HwndHook(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled);
}
