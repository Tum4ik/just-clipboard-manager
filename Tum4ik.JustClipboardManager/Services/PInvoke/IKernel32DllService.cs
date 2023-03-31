using System.Runtime.InteropServices;

namespace Tum4ik.JustClipboardManager.Services.PInvoke;
internal interface IKernel32DllService
{
  int GlobalAddAtom(string name);

  int GlobalDeleteAtom(int nAtom);
}


internal class Kernel32DllService : IKernel32DllService
{
  public int GlobalAddAtom(string name)
  {
    return _GlobalAddAtom(name);
  }

  public int GlobalDeleteAtom(int nAtom)
  {
    return _GlobalDeleteAtom(nAtom);
  }


  [DllImport("kernel32.dll", EntryPoint = "GlobalAddAtom", CharSet = CharSet.Unicode)]
  [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
  private static extern int _GlobalAddAtom(string name);

  [DllImport("kernel32.dll", EntryPoint = "GlobalDeleteAtom")]
  [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
  private static extern int _GlobalDeleteAtom(int nAtom);
}
