using JustClipboardManager.Application.Services;
using JustClipboardManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JustClipboardManager.Infrastructure.Services;

internal class DatabaseMigrator : IDatabaseMigrator
{
  private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

  public DatabaseMigrator(IDbContextFactory<AppDbContext> dbContextFactory)
  {
    _dbContextFactory = dbContextFactory;
  }

  public void Migrate()
  {
    using var dbContext = _dbContextFactory.CreateDbContext();
    dbContext.Database.Migrate();
  }
}
