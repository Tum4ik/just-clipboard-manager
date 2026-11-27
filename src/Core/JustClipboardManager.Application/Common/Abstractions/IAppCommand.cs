using Google.Protobuf.WellKnownTypes;

namespace JustClipboardManager.Application.Common.Abstractions;

public interface IAppCommand
{
  Task<Any> ExecuteAsync(Any parameter, CancellationToken cancellationToken = default);
}
