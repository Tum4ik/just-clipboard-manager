using Prism.Modularity;

namespace Tum4ik.JustClipboardManager.PluginDevKit;
public interface IPluginModule : IModule
{
  Guid Id { get; }
  string Name { get; }
  Version Version { get; }
  string? Author { get; }
  string? Description { get; }
}
