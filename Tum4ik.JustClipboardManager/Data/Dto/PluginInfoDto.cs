namespace Tum4ik.JustClipboardManager.Data.Dto;
internal abstract class PluginInfoDto
{
  public required string Id { get; init; }
  public required string Name { get; init; }
  public required Version Version { get; init; }
  public string? Author { get; init; }
  public string? AuthorEmail { get; init; }
  public string? Description { get; init; }
}
