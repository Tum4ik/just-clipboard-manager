using System.Runtime.InteropServices;

namespace Tum4ik.JustClipboardManager.Services.PInvoke.ParameterModels;

/// <summary>
/// The RECT structure defines the coordinates of the upper-left and lower-right corners of a rectangle.
/// </summary>
/// <remarks>
/// By convention, the right and bottom edges of the rectangle are normally considered exclusive.
/// In other words, the pixel whose coordinates are ( right, bottom ) lies immediately outside of the the rectangle.
/// For example, when RECT is passed to the FillRect function, the rectangle is filled up to, but not including,
/// the right column and bottom row of pixels. This structure is identical to the RECTL structure.
/// </remarks>
[StructLayout(LayoutKind.Sequential)]
internal struct RectStruct
{
  /// <summary>
  /// The x-coordinate of the upper-left corner of the rectangle.
  /// </summary>
  public int Left { get; set; }

  /// <summary>
  /// The y-coordinate of the upper-left corner of the rectangle.
  /// </summary>
  public int Top { get; set; }

  /// <summary>
  /// The x-coordinate of the lower-right corner of the rectangle.
  /// </summary>
  public int Right { get; set; }

  /// <summary>
  /// The y-coordinate of the lower-right corner of the rectangle.
  /// </summary>
  public int Bottom { get; set; }
}
