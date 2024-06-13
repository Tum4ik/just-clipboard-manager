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


  public async Task UpdateIsInstalledAsync(Guid id, bool isInstalled)
  {
    using var dbContext = await _dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);
    await dbContext.Plugins
      .Where(p => p.Id == id)
      .ExecuteUpdateAsync(x => x.SetProperty(p => p.IsInstalled, isInstalled))
      .ConfigureAwait(false);
  }


  public async Task UpdateIsEnabledAsync(Guid id, bool isEnabled)
  {
    using var dbContext = await _dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);
    await dbContext.Plugins
      .Where(p => p.Id == id)
      .ExecuteUpdateAsync(x => x.SetProperty(p => p.IsEnabled, isEnabled))
      .ConfigureAwait(false);
  }


  public async Task UpdateAsync(Guid id, Version version, bool isInstalled)
  {
    using var dbContext = await _dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);
    await dbContext.Plugins
      .Where(p => p.Id == id)
      .ExecuteUpdateAsync(x => x
        .SetProperty(p => p.Version, version.ToString())
        .SetProperty(p => p.IsInstalled, isInstalled)
      )
      .ConfigureAwait(false);
  }


  public async Task DeleteByIdAsync(Guid id)
  {
    using var dbContext = await _dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);
    await dbContext.Plugins
      .Where(p => p.Id == id)
      .ExecuteDeleteAsync()
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


  public async Task<bool> ExistsAsync(Guid id, Version version)
  {
    var versionStr = version.ToString();
    using var dbContext = await _dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);
    return await dbContext.Plugins.AnyAsync(p => p.Id == id && p.Version == versionStr).ConfigureAwait(false);
  }


  public async Task<bool> IsInstalledAsync(Guid id)
  {
    using var dbContext = await _dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);
    return await dbContext.Plugins.AnyAsync(p => p.Id == id && p.IsInstalled).ConfigureAwait(false);
  }


  public async Task<bool> IsEnabledAsync(Guid id)
  {
    using var dbContext = await _dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);
    return await dbContext.Plugins.AnyAsync(p => p.Id == id && p.IsEnabled).ConfigureAwait(false);
  }


  public async Task<bool> IsInstalledAndEnabledAsync(Guid id)
  {
    using var dbContext = await _dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);
    return await dbContext.Plugins.AnyAsync(p => p.Id == id && p.IsInstalled && p.IsEnabled).ConfigureAwait(false);
  }
}
