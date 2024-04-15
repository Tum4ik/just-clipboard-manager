using System.IO;

namespace Tum4ik.JustClipboardManager.Services;
internal interface IUpdateService
{
  Task<CheckUpdatesResult> CheckForUpdatesAsync();
  Task<FileInfo?> DownloadUpdatesAsync(Uri downloadLink, IProgress<int>? progress, CancellationToken cancellationToken);
  Task<UpdateResult> SilentUpdateAsync();
}


internal record CheckUpdatesResult(bool NewVersionIsAvailable)
{
  public Version? LatestVersion { get; init; }
  public Uri? DownloadLink { get; init; }
  public string? ReleaseNotes { get; init; }
}


internal enum UpdateResult
{
  NotRequired, Started
}
