using JustClipboardManager.Application.Common.Abstractions;
using JustClipboardManager.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace JustClipboardManager.Application.Commands;

internal class CreateClipCommand : AppCommand<None, None>
{
  private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

  public CreateClipCommand(IDbContextFactory<AppDbContext> dbContextFactory)
  {
    _dbContextFactory = dbContextFactory;
  }

  protected override Task<None> ExecuteAsync(None parameters, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }
}
