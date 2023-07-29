using Microsoft.EntityFrameworkCore;

namespace Tum4ik.JustClipboardManager.Data.Models;

[Index(nameof(SearchLabel))]
internal class Clip
{
  public int Id { get; set; }

  public required string PluginId { get; set; }
  public required byte[] RepresentationData { get; set; }
  public string? SearchLabel { get; set; }
  public DateTime ClippedAt { get; set; }

  public virtual ICollection<FormattedDataObject> FormattedDataObjects { get; set; } = null!;
}
