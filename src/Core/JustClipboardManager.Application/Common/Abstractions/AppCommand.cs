using System.Text.Json;
using System.Text.Json.Nodes;

namespace JustClipboardManager.Application.Common.Abstractions;

internal abstract class AppCommand<TParameters, TResult> : IAppCommand
{
  public async Task<JsonNode?> ExecuteAsync(JsonNode? parameters, CancellationToken cancellationToken = default)
  {
    TParameters? deserializedParameters = default;
    if (parameters is not null)
    {
      deserializedParameters = parameters.Deserialize<TParameters>();
    }

    var result = await ExecuteAsync(deserializedParameters, cancellationToken);

    JsonNode? serializedResult = null;
    if (result is not null)
    {
      serializedResult = JsonSerializer.SerializeToNode(result);
    }

    return serializedResult;
  }


  protected abstract Task<TResult?> ExecuteAsync(TParameters? parameters, CancellationToken cancellationToken = default);
}
