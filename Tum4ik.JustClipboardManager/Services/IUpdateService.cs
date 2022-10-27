using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Tum4ik.JustClipboardManager.Services;
internal interface IUpdateService
{
  Task<CheckUpdatesResult> CheckForUpdatesAsync();
  Task<FileInfo?> DownloadUpdatesAsync(Uri downloadLink, IProgress<int>? progress, CancellationToken cancellationToken);
  void InstallUpdates(FileInfo exeFile);
  void SilentUpdate();
}


public record CheckUpdatesResult(bool NewVersionIsAvailable)
{
  public Version? LatestVersion { get; init; }
  public Uri? DownloadLink { get; init; }
  public string? ReleaseNotes { get; init; }
}
