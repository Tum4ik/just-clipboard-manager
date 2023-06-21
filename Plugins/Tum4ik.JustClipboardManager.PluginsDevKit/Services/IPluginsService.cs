namespace Tum4ik.JustClipboardManager.PluginDevKit.Services;
public interface IPluginsService
{
  void Register(Guid id);
  void Unregister(Guid id);
  IReadOnlyCollection<IPlugin> Plugins();
  IPlugin? GetPlugin(Guid id);
}
