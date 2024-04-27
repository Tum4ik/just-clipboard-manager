namespace Tum4ik.JustClipboardManager.Data.Models;

internal class Plugin
{
  public Guid Id { get; set; }

  public required string Name { get; set; }
  public required string Version { get; set; }

  public virtual ICollection<PluginFile> Files { get; set; } = null!;
}
