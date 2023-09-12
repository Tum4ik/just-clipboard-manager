using System.Reflection;
using System.Windows;
using Tum4ik.JustClipboardManager.PluginDevKit.Attributes;
using Tum4ik.JustClipboardManager.PluginDevKit.Models;

namespace Tum4ik.JustClipboardManager.PluginDevKit;
public abstract class Plugin<T> : IPlugin
  where T : FrameworkElement
{
  private string? _id;
  public string? Id => _id ??= GetId(GetType());

  public abstract string Format { get; }
  public DataTemplate RepresentationDataDataTemplate { get; } = new() { VisualTree = new(typeof(T)) };
  public abstract ClipData? ProcessData(object data);
  public abstract object RestoreData(byte[] bytes, string? additionalInfo);
  public abstract object RestoreRepresentationData(byte[] bytes, string? additionalInfo);


  internal static string? GetId(Type pluginType)
  {
    return pluginType.GetCustomAttribute<PluginAttribute>(true)?.Id;
  }
}
