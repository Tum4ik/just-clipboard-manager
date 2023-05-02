using System.Diagnostics;
using Prism.Events;
using Tum4ik.JustClipboardManager.Events;
using Tum4ik.JustClipboardManager.Ioc.Wrappers;
using Tum4ik.JustClipboardManager.Services;
using Tum4ik.JustClipboardManager.Services.Translation;
using Tum4ik.JustClipboardManager.ViewModels.Main;

namespace Tum4ik.JustClipboardManager.UnitTests.ViewModels.Main;
public class AboutViewModelTests
{
  private readonly Mock<IInfoService> _infoServiceMock = new();
  private readonly Mock<IEnvironment> _environmentMock = new();
  private readonly Mock<IProcess> _processMock = new();
  private readonly Mock<IClipboard> _clipboardMock = new();
  private readonly AboutViewModel _testeeVm;

  public AboutViewModelTests()
  {
    var eventAggregatorMock = new Mock<IEventAggregator>();
    eventAggregatorMock.Setup(ea => ea.GetEvent<LanguageChangedEvent>()).Returns(Mock.Of<LanguageChangedEvent>());
    _testeeVm = new(
      Mock.Of<ITranslationService>(),
      eventAggregatorMock.Object,
      _infoServiceMock.Object,
      _environmentMock.Object,
      _processMock.Object,
      _clipboardMock.Object
    );
  }


  [Theory]
  [InlineData("1.2.3.4", true)]
  [InlineData("4.3.2.1", false)]
  internal void VersionTest(string versionStr, bool is64BitProcess)
  {
    var version = new Version(versionStr);
    _infoServiceMock.Setup(i => i.Version).Returns(version);
    _environmentMock.Setup(e => e.Is64BitProcess).Returns(is64BitProcess);
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
    _processMock.VerifyNoOtherCalls();
  }


  [Fact]
  internal void OpenLink_LinkIsOk_LinkIsPerformed()
  {
    var link = "https://github.com";
    _testeeVm.OpenLinkCommand.Execute(link);
    _processMock.Verify(
      p => p.Start(It.Is<ProcessStartInfo>(psi => psi.FileName == link && psi.UseShellExecute)),
      Times.Once
    );
  }


  [Fact]
  internal void CopyEmailToClipboardTest()
  {
    _testeeVm.CopyEmailToClipboardCommand.Execute(null);
    _clipboardMock.Verify(c => c.SetText(_testeeVm.Email), Times.Once);
  }
}
