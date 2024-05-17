using Tum4ik.JustClipboardManager.Data.Models;

namespace Tum4ik.JustClipboardManager.Data.Repositories;
internal interface IPluginRepository
{
  Task AddAsync(Plugin plugin);
  IAsyncEnumerable<Plugin> GetInstalledPluginsAsync();
  IAsyncEnumerable<PluginFile> GetUninstalledPluginsFilesAsync();
  Task UpdateIsInstalledAsync(Guid id, bool isInstalled);
  Task DeleteByIdAsync(Guid id);
  Task DeleteUninstalledPluginsAsync();
  Task<bool> ExistsAsync(Guid id);
}
