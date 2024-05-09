using Microsoft.EntityFrameworkCore;
using Tum4ik.JustClipboardManager.Data.Models;

namespace Tum4ik.JustClipboardManager.Data.Repositories;
internal class PluginRepository : IPluginRepository
{
  private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

  public PluginRepository(IDbContextFactory<AppDbContext> dbContextFactory)
  {
    _dbContextFactory = dbContextFactory;
  }


  public async Task AddAsync(Plugin plugin)
  {
    using var dbContext = await _dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);
    await dbContext.Plugins.AddAsync(plugin).ConfigureAwait(false);
    await dbContext.SaveChangesAsync().ConfigureAwait(false);
  }


  public async IAsyncEnumerable<PluginFile> GetUninstalledPluginsFilesAsync()
  {
    using var dbContext = await _dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);
    var files = dbContext.Plugins
      .Where(p => !p.IsInstalled)
      .Include(p => p.Files)
      .SelectMany(p => p.Files)
      .AsAsyncEnumerable();
    await foreach (var file in files.ConfigureAwait(false))
    {
      yield return file;
    }
  }


  public async Task UpdateIsInstalledAsync(Guid id, bool isInstalled)
  {
    using var dbContext = await _dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);
    await dbContext.Plugins
      .Where(p => p.Id == id)
      .ExecuteUpdateAsync(x => x.SetProperty(p => p.IsInstalled, isInstalled))
      .ConfigureAwait(false);
  }


  public async Task DeleteUninstalledPluginsAsync()
  {
    using var dbContext = await _dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);
    await dbContext.Plugins.Where(p => !p.IsInstalled).ExecuteDeleteAsync().ConfigureAwait(false);
  }


  public async Task<bool> ExistsAsync(Guid id)
  {
    using var dbContext = await _dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);
    return await dbContext.Plugins.AnyAsync(p => p.Id == id).ConfigureAwait(false);
  }
}
