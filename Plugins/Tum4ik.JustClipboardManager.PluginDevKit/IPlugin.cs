using System.Windows;
using Tum4ik.JustClipboardManager.PluginDevKit.Models;

namespace Tum4ik.JustClipboardManager.PluginDevKit;
public interface IPlugin
{
  Guid Id { get; }
  IReadOnlyCollection<string> Formats { get; }
  DataTemplate RepresentationDataDataTemplate { get; }
  ClipData? ProcessData(IDataObject dataObject);
  object? RestoreData(byte[] bytes, string? additionalInfo);
  object? RestoreRepresentationData(byte[] bytes, string? additionalInfo);
}
