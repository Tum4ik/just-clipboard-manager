using JustClipboardManager.Application.Services;
using JustClipboardManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JustClipboardManager.Infrastructure.Services;

internal class DatabaseMigrator : IDatabaseMigrator
{
  private readonly IDbContextFactory<InfraDbContext> _dbContextFactory;

  public DatabaseMigrator(IDbContextFactory<InfraDbContext> dbContextFactory)
  {
    _dbContextFactory = dbContextFactory;
  }

  public async Task MigrateAsync(CancellationToken cancellationToken = default)
  {
    using var dbContext = _dbContextFactory.CreateDbContext();
    await dbContext.Database.MigrateAsync(cancellationToken);
  }
}
