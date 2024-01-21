using Microsoft.UI.Xaml;
using Tum4ik.JustClipboardManager.PluginDevKit.Models;
using Windows.ApplicationModel.DataTransfer;

namespace Tum4ik.JustClipboardManager.PluginDevKit;
public interface IPlugin
{
  string? Id { get; }

  IReadOnlyCollection<string> Formats { get; }

  DataTemplate RepresentationDataDataTemplate { get; }

  Task<ClipData> ProcessDataAsync(DataPackageView dataPackageView);

  object? RestoreData(byte[] bytes, string? additionalInfo);

  object? RestoreRepresentationData(byte[] bytes, string? additionalInfo);
}
