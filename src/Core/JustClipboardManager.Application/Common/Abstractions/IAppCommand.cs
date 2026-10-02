using System.Text.Json.Nodes;

namespace JustClipboardManager.Application.Common.Abstractions;

public interface IAppCommand
{
  Task<string?> ExecuteAsync(string? parameter, CancellationToken cancellationToken = default);
}
