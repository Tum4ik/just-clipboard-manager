using System;

namespace Tum4ik.JustClipboardManager.Services;
internal interface IHookService
{
  nint HwndHook(nint hWnd, int msg, nint wParam, nint lParam, ref bool handled);
}
