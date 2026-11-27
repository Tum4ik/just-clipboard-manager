using Google.Protobuf.WellKnownTypes;
using Google.Protobuf;

namespace JustClipboardManager.Application.Common.Abstractions;

internal abstract class AppCommand<TParameter, TResult> : IAppCommand
  where TParameter : class, IMessage<TParameter>, new()
  where TResult : class, IMessage<TResult>, new()
{
  public async Task<Any> ExecuteAsync(Any parameter, CancellationToken cancellationToken = default)
  {
    var unpackedParameter = parameter.Unpack<TParameter>();
    var result = await ExecuteAsync(unpackedParameter, cancellationToken);
    var packedResult = Any.Pack(result);

    return packedResult;
  }

  protected abstract Task<TResult> ExecuteAsync(TParameter parameter, CancellationToken cancellationToken = default);
}
