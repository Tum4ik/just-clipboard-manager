using System.Diagnostics;
using Prism.Events;
using Tum4ik.JustClipboardManager.Ioc.Wrappers;
using Tum4ik.JustClipboardManager.PluginDevKit.Events;
using Tum4ik.JustClipboardManager.Services;
using Tum4ik.JustClipboardManager.Services.Translation;
using Tum4ik.JustClipboardManager.ViewModels.Main;

namespace Tum4ik.JustClipboardManager.UnitTests.ViewModels.Main;
public class AboutViewModelTests
{
  private readonly IInfoService _infoService = Substitute.For<IInfoService>();
  private readonly IEnvironment _environment = Substitute.For<IEnvironment>();
  private readonly IProcess _process = Substitute.For<IProcess>();
  private readonly IClipboard _clipboard = Substitute.For<IClipboard>();
  private readonly AboutViewModel _testeeVm;

  public AboutViewModelTests()
  {
    var eventAggregator = Substitute.For<IEventAggregator>();
    eventAggregator.GetEvent<LanguageChangedEvent>().Returns(Substitute.For<LanguageChangedEvent>());
    _testeeVm = new(
      Substitute.For<ITranslationService>(),
      eventAggregator,
      _infoService,
      _environment,
      _process,
      _clipboard
    );
  }


  [Theory]
  [InlineData("1.2.3.4", true)]
  [InlineData("4.3.2.1", false)]
  internal void VersionTest(string versionStr, bool is64BitProcess)
  {
    var version = new Version(versionStr);
    _infoService.Version.Returns(version);
    _environment.Is64BitProcess.Returns(is64BitProcess);
    var actualVersion = _testeeVm.Version;
    var arch = is64BitProcess ? "64" : "32";
    actualVersion.Should().Be($"{versionStr} ({arch}-bit)");
  }


  [Theory]
  [InlineData(null)]
  [InlineData("")]
  [InlineData("   ")]
  internal void OpenLink_LinkIsNullOrWhitespace_NoAction(string? link)
  {
    _testeeVm.OpenLinkCommand.Execute(link);
    _process.ReceivedCalls().Any().Should().BeFalse();
  }


  [Fact]
  internal void OpenLink_LinkIsOk_LinkIsPerformed()
  {
    var link = "https://github.com";
    _testeeVm.OpenLinkCommand.Execute(link);
    _process.Received(1).Start(Arg.Is<ProcessStartInfo>(psi => psi.FileName == link && psi.UseShellExecute));
  }


  [Fact]
  internal void CopyEmailToClipboardTest()
  {
    _testeeVm.CopyEmailToClipboardCommand.Execute(null);
    _clipboard.Received(1).SetText(_testeeVm.Email);
  }
}
