using System.Runtime.InteropServices;

namespace Tum4ik.JustClipboardManager.Services.PInvoke.ParameterModels;

/// <summary>
/// Defines the x- and y-coordinates of a point.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal struct POINT
{
  /// <summary>
  /// Specifies the x-coordinate of the point.
  /// </summary>
  public int x;

  /// <summary>
  /// Specifies the y-coordinate of the point.
  /// </summary>
  public int y;
}
