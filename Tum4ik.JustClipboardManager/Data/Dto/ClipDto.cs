namespace Tum4ik.JustClipboardManager.Data.Dto;
internal class ClipDto
{
  public required int Id { get; set; }
  public required Guid PluginId { get; set; }
  public required object RepresentationData { get; set; }
  public string? SearchLabel { get; set; }
}
