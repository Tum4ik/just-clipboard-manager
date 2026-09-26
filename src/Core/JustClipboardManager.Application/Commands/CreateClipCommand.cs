using JustClipboardManager.Application.Common.Abstractions;
using JustClipboardManager.Domain.Common;
using JustClipboardManager.Domain.Entities;
using JustClipboardManager.PluginDevKit.Core;
using Microsoft.EntityFrameworkCore;

namespace JustClipboardManager.Application.Commands;

internal class CreateClipCommand : AppCommand<ProcessDataResult, None>
{
  private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

  public CreateClipCommand(IDbContextFactory<AppDbContext> dbContextFactory)
  {
    _dbContextFactory = dbContextFactory;
  }

  protected override async Task<None> ExecuteAsync(ProcessDataResult? parameters, CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(parameters);

    

    return None.Value;
  }
}
