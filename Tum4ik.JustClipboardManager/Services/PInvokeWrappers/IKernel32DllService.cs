using Windows.Win32;

namespace Tum4ik.JustClipboardManager.Services.PInvokeWrappers;
internal interface IKernel32DllService
{
  /// <inheritdoc cref="PInvoke.GlobalAddAtom(string)"/>
  ushort GlobalAddAtom(string lpString) => PInvoke.GlobalAddAtom(lpString);

  /// <inheritdoc cref="PInvoke.GlobalDeleteAtom(ushort)"/>
  ushort GlobalDeleteAtom(ushort nAtom) => PInvoke.GlobalDeleteAtom(nAtom);
}


internal class Kernel32DllService : IKernel32DllService { }
