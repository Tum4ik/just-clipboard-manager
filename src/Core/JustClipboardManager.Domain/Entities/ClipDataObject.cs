namespace JustClipboardManager.Domain.Entities;

public class ClipDataObject
{
  public long Id { get; set; }

  public required int FormatId { get; set; }
  public required byte[] Data { get; set; }

  public long ClipId { get; set; }
}
