namespace Tum4ik.JustClipboardManager.Data.Models;

internal class Plugin
{
  public Guid Id { get; set; }

  public required string Name { get; set; }
  public required string Version { get; set; }
  public string? Author { get; set; }
  public string? AuthorEmail { get; set; }
  public string? Description { get; set; }
  public bool IsInstalled { get; set; } = true;
  public bool IsEnabled { get; set; } = true;
  public required string FilesDirectory { get; set; }
}
