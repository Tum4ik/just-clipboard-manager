namespace JustClipboardManager.Domain.Entities;

public class Clip
{
  public long Id { get; set; }

  public required byte[] PreviewData { get; set; }
  public string? SearchLabel { get; set; }
  public DateTime ClippedAt { get; set; }

  public virtual ICollection<ClipDataObject> ClipDataObjects { get; } = [];
}
