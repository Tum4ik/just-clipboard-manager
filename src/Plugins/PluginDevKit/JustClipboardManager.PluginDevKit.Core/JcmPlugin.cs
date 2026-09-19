using System.Reflection;

namespace JustClipboardManager.PluginDevKit.Core;

public abstract class JcmPlugin<TClipboardContent>
{
  private Guid? _id;
  public Guid Id => _id ??= GetId(GetType());

  private static Guid GetId(Type pluginType)
  {
    var attr = pluginType.GetCustomAttribute<PluginAttribute>();
    return Guid.Parse(attr!.Id);
  }


  public abstract object? ProcessData(TClipboardContent dataPackageView);
}
