namespace Tum4ik.JustClipboardManager.Data.Models;

internal class FormattedDataObject
{
  public int Id { get; set; }

  public required byte[] Data { get; set; }
  public required string DataDotnetType { get; set; }
  public required string Format { get; set; }
  public required int FormatOrder { get; set; }

  public virtual Clip Clip { get; set; } = null!;
}
