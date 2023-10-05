using Prism.Modularity;

namespace Tum4ik.JustClipboardManager.Services;
internal interface ILoadableDirectoryModuleCatalog : IModuleCatalog
{
  void Load();
}


internal class LoadableDirectoryModuleCatalog : DirectoryModuleCatalog, ILoadableDirectoryModuleCatalog
{
}
