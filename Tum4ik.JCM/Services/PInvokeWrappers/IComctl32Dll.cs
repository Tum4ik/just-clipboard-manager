using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Shell;

namespace Tum4ik.JustClipboardManager.Services.PInvokeWrappers;
internal interface IComctl32Dll
{
  /// <inheritdoc cref="PInvoke.SetWindowSubclass(HWND, SUBCLASSPROC, nuint, nuint)"/>
  bool SetWindowSubclass(HWND hWnd, SUBCLASSPROC pfnSubclass, nuint uIdSubclass, nuint dwRefData) =>
    PInvoke.SetWindowSubclass(hWnd, pfnSubclass, uIdSubclass, dwRefData);

  /// <inheritdoc cref="PInvoke.DefSubclassProc(HWND, uint, WPARAM, LPARAM)"/>
  LRESULT DefSubclassProc(HWND hWnd, uint uMsg, WPARAM wParam, LPARAM lParam) =>
    PInvoke.DefSubclassProc(hWnd, uMsg, wParam, lParam);
}


internal sealed class Comctl32Dll : IComctl32Dll { }
