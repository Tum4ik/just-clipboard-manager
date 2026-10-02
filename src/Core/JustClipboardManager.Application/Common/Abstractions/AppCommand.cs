using System.Text.Json;
using JustClipboardManager.Domain.Common;

namespace JustClipboardManager.Application.Common.Abstractions;

internal abstract class AppCommand<TParameter, TResult> : IAppCommand
{
  public async Task<string?> ExecuteAsync(string? parameter, CancellationToken cancellationToken = default)
  {
    TParameter? parsedParameter = default;
    if (parameter is not null && typeof(TParameter) != typeof(None))
    {
      var parsable = typeof(TParameter).GetInterfaces()
        .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IParsable<>));
      if (parsable is null)
      {
        parsedParameter = JsonSerializer.Deserialize<TParameter>(parameter);
      }
      else
      {
        var parseMethod = parsable.GetMethod("Parse", [typeof(string), typeof(IFormatProvider)])!;
        parsedParameter = (TParameter)parseMethod.Invoke(null, [parameter])!;
      }
    }

    var result = await ExecuteAsync(default(TParameter)!, cancellationToken);

    string? serializedResult = null;
    if (result is not null && typeof(TResult) != typeof(None))
    {
      if (result is string stringResult)
      {
        serializedResult = stringResult;
      }
      else if (typeof(TResult).GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IParsable<>)))
      {
        serializedResult = result.ToString();
      }
      else
      {
        serializedResult = JsonSerializer.Serialize(result);
      }
    }

    return serializedResult;
  }

  protected abstract Task<TResult> ExecuteAsync(TParameter parameters, CancellationToken cancellationToken = default);
}
