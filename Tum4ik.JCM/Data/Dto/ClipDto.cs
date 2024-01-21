namespace Tum4ik.JustClipboardManager.Data.Dto;
internal class ClipDto
{
  public required int Id { get; set; }
  public required string PluginId { get; set; }
  public object? RepresentationData { get; set; }
  public string? SearchLabel { get; set; }
}
