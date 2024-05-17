using Prism.Modularity;

namespace Tum4ik.JustClipboardManager.Services;
internal interface ILoadableDirectoryModuleCatalog : IModuleCatalog
{
  PluginInfo? GetPluginInfo(Guid id);
  void Load();
}
