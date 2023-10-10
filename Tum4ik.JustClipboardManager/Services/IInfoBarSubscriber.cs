using Tum4ik.JustClipboardManager.Controls;
using Tum4ik.JustClipboardManager.PluginDevKit.Services;

namespace Tum4ik.JustClipboardManager.Services;

internal interface IInfoBarSubscriber
{
  event Action<InfoBarPayload> InfoReceived;
}


internal class InfoBarPayload
{
  public InfoBarSeverity Severity { get; init; }
  public InfoBarActionType ActionType { get; init; }
  public string? Title { get; init; }
  public required string Body { get; init; }
  public string? ActionText { get; init; }
  public Action<InfoBarResult>? Callback { get; init; }
}
