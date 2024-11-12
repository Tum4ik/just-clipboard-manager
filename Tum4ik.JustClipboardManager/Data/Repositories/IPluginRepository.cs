using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Tum4ik.JustClipboardManager.Data.Models;

namespace Tum4ik.JustClipboardManager.Data.Repositories;
internal interface IPluginRepository
{
  Task AddAsync(Plugin plugin);
  IAsyncEnumerable<Plugin> GetAllAsync();
  IAsyncEnumerable<Plugin> GetInstalledPluginsAsync();
  IAsyncEnumerable<Plugin> GetUninstalledPluginsAsync();
  Task UpdateAsync(Guid id, Expression<Func<SetPropertyCalls<Plugin>, SetPropertyCalls<Plugin>>> updates);
  Task DeleteUninstalledPluginsAsync();
  Task<bool> ExistsAsync(Guid id);
  Task<bool> IsInstalledAsync(Guid id);
  Task<bool> IsInstalledAndEnabledAsync(Guid id);
}
