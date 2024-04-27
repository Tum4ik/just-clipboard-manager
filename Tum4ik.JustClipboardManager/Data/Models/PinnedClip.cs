namespace Tum4ik.JustClipboardManager.Data.Models;

internal class PinnedClip
{
  public int Id { get; set; }

  public int Order { get; set; }
  public virtual Clip Clip { get; set; } = null!;
}
