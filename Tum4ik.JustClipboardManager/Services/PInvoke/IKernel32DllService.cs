using System.Runtime.InteropServices;

namespace Tum4ik.JustClipboardManager.Services.PInvoke;
internal interface IKernel32DllService
{
  int GlobalAddAtom(string name) => Native_GlobalAddAtom(name);

  int GlobalDeleteAtom(int nAtom) => Native_GlobalDeleteAtom(nAtom);


  [DllImport("kernel32.dll", EntryPoint = "GlobalAddAtom", CharSet = CharSet.Unicode)]
  [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
  private static extern int Native_GlobalAddAtom(string name);


  [DllImport("kernel32.dll", EntryPoint = "GlobalDeleteAtom")]
  [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
  private static extern int Native_GlobalDeleteAtom(int nAtom);
}


internal class Kernel32DllService : IKernel32DllService { }
