using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Http;
using Octokit;
using Prism.Services.Dialogs;
using Tum4ik.JustClipboardManager.Constants;
using FileMode = System.IO.FileMode;

namespace Tum4ik.JustClipboardManager.Services;
internal class UpdateService : IUpdateService
{
  private readonly IGitHubClient _gitHubClient;
  private readonly IHub _sentryHub;
  private readonly IDialogService _dialogService;

  public UpdateService(IGitHubClient gitHubClient,
                       IHub sentryHub,
                       IDialogService dialogService)
  {
    _gitHubClient = gitHubClient;
    _sentryHub = sentryHub;
    _dialogService = dialogService;
  }


  public async Task<CheckUpdatesResult> CheckForUpdatesAsync()
  {
    try
    {
      var latestRelease = await _gitHubClient.Repository
        .Release
        .GetLatest("Tum4ik", "just-clipboard-manager")
        .ConfigureAwait(true);
      if (Version.TryParse(latestRelease.TagName, out var latestReleaseVersion))
      {
        if (latestReleaseVersion >= new Version(4, 0, 0))
        {
          _dialogService.Show(DialogNames.MajorUpdateAvailableDialog);
        }
      }
      else
      {
        _sentryHub.CaptureMessage(
          "Parse version problem when check for updates.",
          scope => scope.AddBreadcrumb(
            message: "",
            category: "info",
            type: "info",
            dataPair: ("Lates release tag", latestRelease.TagName)
          ),
          SentryLevel.Error
        );
      }
    }
    catch (TaskCanceledException e)
    {
      _sentryHub.CaptureException(e, scope => scope.AddBreadcrumb(
        message: "When checking for updates",
        type: "info"
      ));
    }
    catch (HttpRequestException)
    {
      // TODO: log request problem
    }
    catch (ApiException e)
    {
      _sentryHub.CaptureException(e, scope => scope.AddBreadcrumb(
        message: "When checking for updates via GitHub API",
        type: "info"
      ));
    }

    return new(false);
  }


  public async Task<FileInfo?> DownloadUpdatesAsync(Uri downloadLink,
                                                    IProgress<int>? progress,
                                                    CancellationToken cancellationToken)
  {
    using var httpClient = new HttpClient();
    try
    {
      using var response = await httpClient
        .GetAsync(downloadLink, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
        .ConfigureAwait(false);
      response.EnsureSuccessStatusCode();

      var exeFilePath = Path.Combine(Path.GetTempPath(), downloadLink.Segments.Last());
      var buffer = new byte[8192];
      var totalBytes = response.Content.Headers.ContentLength;
      var totalBytesDownloaded = 0L;

      using var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
      using var fileStream =
        new FileStream(exeFilePath, FileMode.Create, FileAccess.Write, FileShare.None, buffer.Length, true);

      var prevPercentage = 0;
      while (true)
      {
        var bytesDownloaded = await contentStream
          .ReadAsync(buffer.AsMemory(0, buffer.Length), cancellationToken)
          .ConfigureAwait(false);
        if (bytesDownloaded == 0)
        {
          progress?.Report(100);
          break;
        }

        await fileStream.WriteAsync(buffer.AsMemory(0, bytesDownloaded), cancellationToken).ConfigureAwait(false);
        totalBytesDownloaded += bytesDownloaded;

        int currentPercentage;
        if (totalBytes.HasValue)
        {
          currentPercentage = GetDownloadPercentage(totalBytes.Value, totalBytesDownloaded);
        }
        else
        {
          currentPercentage = 0;
        }

        if (currentPercentage > prevPercentage)
        {
          prevPercentage = currentPercentage;
          progress?.Report(currentPercentage);
        }
      }

      return new(exeFilePath);
    }
    catch (HttpRequestException)
    {
      // TODO: log request problem
    }
    catch (TaskCanceledException)
    {
      // download canceled
    }
    catch (IOException)
    {
      // aborted connection
    }

    return null;
  }


  public async Task<UpdateResult> SilentUpdateAsync()
  {
    var checkUpdatesResult = await CheckForUpdatesAsync().ConfigureAwait(false);
    if (checkUpdatesResult.NewVersionIsAvailable
        && checkUpdatesResult.DownloadLink is not null)
    {
      var exe = await DownloadUpdatesAsync(checkUpdatesResult.DownloadLink, null, CancellationToken.None)
        .ConfigureAwait(false);
      if (exe is not null)
      {
        InstallUpdates(exe);
        return UpdateResult.Started;
      }
    }

    return UpdateResult.NotRequired;
  }


  private static int GetDownloadPercentage(long totalSize, long downloadedSize)
  {
    return (int) ((double) downloadedSize / totalSize * 100);
  }


  [ExcludeFromCodeCoverage]
  private static void InstallUpdates(FileInfo exeFile)
  {
#if !DEBUG
    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(exeFile.FullName, "/SILENT")
    {
      UseShellExecute = true
    });
#endif
  }
}
