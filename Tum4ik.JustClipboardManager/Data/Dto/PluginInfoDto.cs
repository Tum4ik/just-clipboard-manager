namespace Tum4ik.JustClipboardManager.Data.Dto;
internal abstract class PluginInfoDto
{
  public required Guid Id { get; init; }
  public required string Name { get; init; }
  public required string Version { get; init; }
  public string? Author { get; init; }
  public string? AuthorEmail { get; init; }
  public string? Description { get; init; }
}
