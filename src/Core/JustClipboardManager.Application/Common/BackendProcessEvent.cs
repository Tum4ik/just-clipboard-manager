namespace JustClipboardManager.Application.Common;

public class BackendProcessEvent
{
  public string? Id { get; init; }
  public required string Name { get; init; }
  public string? Payload { get; init; }
}
