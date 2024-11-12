using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
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


  public async IAsyncEnumerable<Plugin> GetAllAsync()
  {
    using var dbContext = await _dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);
    await foreach (var plugin in dbContext.Plugins.AsAsyncEnumerable().ConfigureAwait(false))
    {
      yield return plugin;
    }
  }


  public async IAsyncEnumerable<Plugin> GetInstalledPluginsAsync()
  {
    using var dbContext = await _dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);
    var installedPlugins = dbContext.Plugins.Where(p => p.IsInstalled).AsAsyncEnumerable();
    await foreach (var installedPlugin in installedPlugins.ConfigureAwait(false))
    {
      yield return installedPlugin;
    }
  }


  public async IAsyncEnumerable<Plugin> GetUninstalledPluginsAsync()
  {
    using var dbContext = await _dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);
    var installedPlugins = dbContext.Plugins.Where(p => !p.IsInstalled).AsAsyncEnumerable();
    await foreach (var installedPlugin in installedPlugins.ConfigureAwait(false))
    {
      yield return installedPlugin;
    }
  }


  public async Task UpdateAsync(Guid id, Expression<Func<SetPropertyCalls<Plugin>, SetPropertyCalls<Plugin>>> updates)
  {
    using var dbContext = await _dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);
    await dbContext.Plugins
      .Where(p => p.Id == id)
      .ExecuteUpdateAsync(updates)
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


  public async Task<bool> IsInstalledAsync(Guid id)
  {
    using var dbContext = await _dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);
    return await dbContext.Plugins.AnyAsync(p => p.Id == id && p.IsInstalled).ConfigureAwait(false);
  }


  public async Task<bool> IsInstalledAndEnabledAsync(Guid id)
  {
    using var dbContext = await _dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);
    return await dbContext.Plugins.AnyAsync(p => p.Id == id && p.IsInstalled && p.IsEnabled).ConfigureAwait(false);
  }
}
