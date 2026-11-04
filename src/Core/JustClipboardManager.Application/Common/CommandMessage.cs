namespace JustClipboardManager.Application.Common;

public class CommandMessage
{
  public required string EventId { get; init; }
  public required string CommandName { get; init; }
  public string? CommandParameter { get; init; }
}
