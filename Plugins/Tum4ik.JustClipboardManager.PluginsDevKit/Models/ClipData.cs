namespace Tum4ik.JustClipboardManager.PluginDevKit.Models;

public sealed class ClipData
{
  public required byte[] Data { get; init; }
  public required byte[] RepresentationData { get; init; }
  public string? SearchLabel { get; init; }
}
