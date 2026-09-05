using System.Text.Json.Nodes;

namespace JustClipboardManager.Application.Common.Abstractions;

public interface IAppCommand
{
  Task<JsonNode?> ExecuteAsync(JsonNode? parameters, CancellationToken cancellationToken = default);
}
