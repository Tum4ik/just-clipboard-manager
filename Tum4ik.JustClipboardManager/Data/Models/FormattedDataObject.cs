namespace Tum4ik.JustClipboardManager.Data.Models;

internal class FormattedDataObject
{
  public int Id { get; set; }

  public byte[] Data { get; set; } = null!;
  public string DataDotnetType { get; set; } = null!;
  public string Format { get; set; } = null!;
  public int FormatOrder { get; set; }
}
