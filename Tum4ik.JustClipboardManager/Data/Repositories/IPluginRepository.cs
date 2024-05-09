using Tum4ik.JustClipboardManager.Data.Models;

namespace Tum4ik.JustClipboardManager.Data.Repositories;
internal interface IPluginRepository
{
  Task AddAsync(Plugin plugin);
  IAsyncEnumerable<PluginFile> GetUninstalledPluginsFilesAsync();
  Task UpdateIsInstalledAsync(Guid id, bool isInstalled);
  Task DeleteUninstalledPluginsAsync();
  Task<bool> ExistsAsync(Guid id);
}
