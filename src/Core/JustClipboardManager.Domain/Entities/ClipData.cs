namespace JustClipboardManager.Domain.Entities;

public class ClipData
{
  public long Id { get; set; }

  public string? FormatId { get; set; }
  public required byte[] Data { get; set; }

  public long ClipId { get; set; }
}
