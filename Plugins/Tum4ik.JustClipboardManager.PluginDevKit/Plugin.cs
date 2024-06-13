using System.Windows;
using Tum4ik.JustClipboardManager.PluginDevKit.Models;

namespace Tum4ik.JustClipboardManager.PluginDevKit;

public abstract class Plugin<TVisualTree> : IPlugin
  where TVisualTree : FrameworkElement
{
  public abstract IReadOnlyCollection<string> Formats { get; }
  public DataTemplate RepresentationDataDataTemplate { get; } = new() { VisualTree = new(typeof(TVisualTree))};
  public abstract ClipData? ProcessData(IDataObject dataObject);
  public abstract object? RestoreData(byte[] bytes, string? additionalInfo);
  public abstract object? RestoreRepresentationData(byte[] bytes, string? additionalInfo);
}
