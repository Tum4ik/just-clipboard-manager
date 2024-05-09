using System.Windows;
using Tum4ik.JustClipboardManager.PluginDevKit.Models;

namespace Tum4ik.JustClipboardManager.PluginDevKit;

public abstract class Plugin : IPlugin
{
  public Guid Id { get; internal set; }

  public abstract IReadOnlyCollection<string> Formats { get; }
  public DataTemplate RepresentationDataDataTemplate { get; internal set; } = new();
  public abstract ClipData? ProcessData(IDataObject dataObject);
  public abstract object? RestoreData(byte[] bytes, string? additionalInfo);
  public abstract object? RestoreRepresentationData(byte[] bytes, string? additionalInfo);
}
