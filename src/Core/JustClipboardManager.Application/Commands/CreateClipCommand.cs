using JustClipboardManager.Application.Common.Abstractions;
using JustClipboardManager.Domain.Common;

namespace JustClipboardManager.Application.Commands;

internal class CreateClipCommand : AppCommand<None, None>
{
  protected override Task<None> ExecuteAsync(None parameters, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }
}
