using Tum4ik.JustClipboardManager.Data.Models;

namespace Tum4ik.JustClipboardManager.Data.Repositories;
internal interface IPluginRepository
{
  Task AddAsync(Plugin plugin);
  IAsyncEnumerable<Plugin> GetInstalledPluginsAsync();
  IAsyncEnumerable<Plugin> GetUninstalledPluginsAsync();
  Task UpdateIsInstalledAsync(Guid id, bool isInstalled);
  Task UpdateIsEnabledAsync(Guid id, bool isEnabled);
  Task UpdateAsync(Guid id, Version version, bool isInstalled);
  Task DeleteByIdAsync(Guid id);
  Task DeleteUninstalledPluginsAsync();
  Task<bool> ExistsAsync(Guid id);
  Task<bool> ExistsAsync(Guid id, Version version);
  Task<bool> IsInstalledAsync(Guid id);
  Task<bool> IsEnabledAsync(Guid id);
  Task<bool> IsInstalledAndEnabledAsync(Guid id);
}
