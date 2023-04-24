using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Http;
using Microsoft.AppCenter.Crashes;
using Octokit;
using Tum4ik.JustClipboardManager.Ioc.Wrappers;
using FileMode = System.IO.FileMode;

namespace Tum4ik.JustClipboardManager.Services;
internal class UpdateService : IUpdateService
{
  private readonly IInfoService _infoService;
  private readonly IGitHubClient _gitHubClient;
  private readonly IEnvironment _environment;

  public UpdateService(IInfoService infoService,
                       IGitHubClient gitHubClient,
                       IEnvironment environment)
  {
    _infoService = infoService;
    _gitHubClient = gitHubClient;
    _environment = environment;
  }


  public async Task<CheckUpdatesResult> CheckForUpdatesAsync()
  {
    try
    {
      var latestRelease = await _gitHubClient.Repository
        .Release
        .GetLatest("Tum4ik", "just-clipboard-manager")
        .ConfigureAwait(false);
      if (Version.TryParse(latestRelease.TagName, out var latestReleaseVersion))
      {
        if (latestReleaseVersion > _infoService.Version)
        {
          var osArchitecture = _environment.Is64BitOperatingSystem ? "x64" : "x86";
          var downloadLink = latestRelease.Assets
            .Single(a => a.Name.Contains(osArchitecture, StringComparison.OrdinalIgnoreCase)
                      && a.Name.Contains(".exe", StringComparison.OrdinalIgnoreCase))
            .BrowserDownloadUrl;
          return new(true)
          {
            LatestVersion = latestReleaseVersion,
            DownloadLink = new(downloadLink),
            ReleaseNotes = latestRelease.Body
          };
        }
      }
      else
      {
        Crashes.TrackError(
          new ArgumentException("Parse version problem when check for updates."), new Dictionary<string, string>()
          {
            { "Lates release tag", latestRelease.TagName }
          }
        );
      }
    }
    catch (TaskCanceledException)
    {
      // TODO
    }
    catch (HttpRequestException)
    {
      // TODO: log request problem
    }
    catch (ApiException)
    {
      // TODO: log github api exception
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

    return null;
  }


  [ExcludeFromCodeCoverage]
  public void InstallUpdates(FileInfo exeFile)
  {
#if !DEBUG
    Process.Start(new ProcessStartInfo(exeFile.FullName, "/SILENT") { UseShellExecute = true });
#endif
  }


  public async void SilentUpdate()
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
      }
    }
  }


  private static int GetDownloadPercentage(long totalSize, long downloadedSize)
  {
    return (int) ((double) downloadedSize / totalSize * 100);
  }
}
