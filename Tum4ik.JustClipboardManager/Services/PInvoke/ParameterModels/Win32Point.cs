using System.Runtime.InteropServices;

namespace Tum4ik.JustClipboardManager.Services.PInvoke.ParameterModels;

[StructLayout(LayoutKind.Sequential)]
internal struct Win32Point
{
  public int X { get; set; }
  public int Y { get; set; }
}
