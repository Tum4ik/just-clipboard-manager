using Octokit;
using Tum4ik.JustClipboardManager.Ioc.Wrappers;
using Tum4ik.JustClipboardManager.Services;

namespace Tum4ik.JustClipboardManager.UnitTests.Services;
public class UpdateServiceTests
{
  private readonly IInfoService _infoService = Substitute.For<IInfoService>();
  private readonly IReleasesClient _releasesClient = Substitute.For<IReleasesClient>();
  private readonly IEnvironment _environment = Substitute.For<IEnvironment>();
  private readonly UpdateService _testeeService;

  public UpdateServiceTests()
  {
    var gitHubClient = Substitute.For<IGitHubClient>();
    var repositoriesClient = Substitute.For<IRepositoriesClient>();
    gitHubClient.Repository.Returns(repositoriesClient);
    repositoriesClient.Release.Returns(_releasesClient);
    _testeeService = new(_infoService, gitHubClient, _environment);
  }


  private const string X86 = "x86";
  private const string X64 = "x64";
  [Theory]
  [InlineData(X86)]
  [InlineData(X64)]
  public async Task CheckForUpdates_LatestVersionIsGreaterThanCurrent_UpdateAvailable(string cpuArch)
  {
    var currentVersion = new Version(0, 1, 1, 4);
    const string TagName = "0.1.2.3";
    const string DownloadLink_x86 = $"https://github.com/releases/update-{X86}.exe";
    const string DownloadLink_x64 = $"https://github.com/releases/update-{X64}.exe";
    var assets = new List<ReleaseAsset>
    {
      new("", 0, "", $"JustClipboardManager-{X86}.exe", "", "", "", 0, 0, default, default, DownloadLink_x86, null),
      new("", 0, "", $"JustClipboardManager-{X64}.exe", "", "", "", 0, 0, default, default, DownloadLink_x64, null),
      new("", 0, "", "Other_Asset", "", "", "", 0, 0, default, default, "https://github.com/releases/other", null)
    };
    var body = "Some release notes";
    var release = new Release(
      "", "", "", "", 0, "", TagName, "", "", body, false, false, default, default, null, "", "", assets
    );

    _releasesClient.GetLatest("Tum4ik", "just-clipboard-manager").Returns(release);
    _infoService.Version.Returns(currentVersion);

    _environment.Is64BitOperatingSystem.Returns(cpuArch == X64);

    var checkUpdatesResult = await _testeeService.CheckForUpdatesAsync();

    checkUpdatesResult.NewVersionIsAvailable.Should().BeTrue();
    checkUpdatesResult.LatestVersion.Should().Be(new Version(TagName));
    checkUpdatesResult.ReleaseNotes.Should().Be(body);
    if (cpuArch == X86)
    {
      checkUpdatesResult.DownloadLink?.ToString().Should().Be(DownloadLink_x86);
    }
    else if (cpuArch == X64)
    {
      checkUpdatesResult.DownloadLink?.ToString().Should().Be(DownloadLink_x64);
    }
  }


  [Theory]
  [InlineData("0.1.2.2", "0.1.2.3")] // less
  [InlineData("0.1.2.3", "0.1.2.3")] // equals
  public async Task CheckForUpdates_LatestVersionLessOrEqualsCurrent_UpdateUnavailable(string latestVersion,
                                                                                       string currentVersionStr)
  {
    var currentVersion = new Version(currentVersionStr);
    var release = new Release(
      "", "", "", "", 0, "", latestVersion, "", "", null, false, false, default, default, null, "", "", null
    );

    _releasesClient.GetLatest("Tum4ik", "just-clipboard-manager").Returns(release);
    _infoService.Version.Returns(currentVersion);

    var checkUpdatesResult = await _testeeService.CheckForUpdatesAsync();

    checkUpdatesResult.NewVersionIsAvailable.Should().BeFalse();
    checkUpdatesResult.LatestVersion.Should().BeNull();
    checkUpdatesResult.DownloadLink.Should().BeNull();
    checkUpdatesResult.ReleaseNotes.Should().BeNull();
  }
}
