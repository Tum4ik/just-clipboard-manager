using System.Runtime.InteropServices;

namespace Tum4ik.JustClipboardManager.Services.PInvoke;
internal interface IKernel32DllService
{
  /// <summary>
  /// Adds a character string to the global atom table and returns a unique value (an atom) identifying the string.
  /// </summary>
  /// <param name="lpString">
  /// The null-terminated string to be added. The string can have a maximum size of 255 bytes.
  /// Strings that differ only in case are considered identical. The case of the first string of this name
  /// added to the table is preserved and returned by the GlobalGetAtomName function.
  /// </param>
  /// <returns>
  /// If the function succeeds, the return value is the newly created atom.
  /// If the function fails, the return value is zero.
  /// </returns>
  ushort GlobalAddAtom(string lpString) => _GlobalAddAtom(lpString);

  /// <summary>
  /// Decrements the reference count of a global string atom. If the atom's reference count reaches zero,
  /// removes the string associated with the atom from the global atom table.
  /// </summary>
  /// <param name="nAtom">The atom and character string to be deleted.</param>
  /// <returns>The function always returns (ATOM) 0.</returns>
  ushort GlobalDeleteAtom(ushort nAtom) => _GlobalDeleteAtom(nAtom);


  [DllImport("kernel32.dll", EntryPoint = "GlobalAddAtom", CharSet = CharSet.Unicode)]
  [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
  private static extern ushort _GlobalAddAtom(string lpString);


  [DllImport("kernel32.dll", EntryPoint = "GlobalDeleteAtom")]
  [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
  private static extern ushort _GlobalDeleteAtom(ushort nAtom);
}


internal class Kernel32DllService : IKernel32DllService { }
