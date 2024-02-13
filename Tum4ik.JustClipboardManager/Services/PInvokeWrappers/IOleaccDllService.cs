using System.Runtime.InteropServices;
using Windows.Win32.Foundation;
using Windows.Win32;
using Windows.Win32.UI.WindowsAndMessaging;

namespace Tum4ik.JustClipboardManager.Services.PInvokeWrappers;
internal interface IOleaccDllService
{
  /// <inheritdoc cref="PInvoke.AccessibleObjectFromWindow(HWND, uint, in Guid, out void*)"/>
  unsafe int AccessibleObjectFromWindow(nint hwnd, OBJECT_IDENTIFIER dwId, Guid riid, out object ppvObject)
  {
    var result = PInvoke.AccessibleObjectFromWindow((HWND) hwnd, (uint) dwId, riid, out var ppv);
    ppvObject = Marshal.GetObjectForIUnknown((nint) ppv);
    return result;
  }
}


internal class OleaccDllService : IOleaccDllService { }
