using System.Linq.Expressions;
using IWshRuntimeLibrary;
using Tum4ik.JustClipboardManager.Ioc.Wrappers;
using Tum4ik.JustClipboardManager.Services;
using IFile = Tum4ik.JustClipboardManager.Ioc.Wrappers.IFile;

namespace Tum4ik.JustClipboardManager.UnitTests.Services;
public class ShortcutServiceTests
{
  private readonly Mock<IInfoService> _infoServiceMock = new();
  private readonly Mock<WshShell> _wshShellMock = new();
  private readonly Mock<IFile> _fileMock = new();
  private readonly Mock<IPath> _pathMock = new();
  private readonly Mock<IEnvironment> _environmentMock = new();
  private readonly ShortcutService _testeeService;

  public ShortcutServiceTests()
  {
    _testeeService = new(
      _infoServiceMock.Object,
      _wshShellMock.Object,
      _fileMock.Object,
      _pathMock.Object,
      _environmentMock.Object
    );
  }


  public static IEnumerable<object[]> SpecialFoldersData()
  {
    foreach (var specialFolder in Enum.GetValues<Environment.SpecialFolder>())
    {
      yield return new object[] { specialFolder };
    }
  }


  public static IEnumerable<object[]> SpecialFoldersAndShortcutExistanceData()
  {
    var exists = true;
    foreach (var specialFolder in Enum.GetValues<Environment.SpecialFolder>())
    {
      exists = !exists;
      yield return new object[] { specialFolder, exists };
    }
  }


  [Theory, MemberData(nameof(SpecialFoldersAndShortcutExistanceData))]
  internal void ExistsTest(Environment.SpecialFolder specialFolder, bool shortcutExists)
  {
    const string ProductName = "Just Clipboard Manager";
    const string FolderPath = @"X:\SpecialFolder\Path";
    var start = Path.Combine(FolderPath, ProductName);
    const string End = ".lnk";
    Expression<Func<string, bool>> shortcutPathMatch = s => s.StartsWith(start) && s.EndsWith(End);
    _infoServiceMock.Setup(i => i.ProductName).Returns(ProductName);
    _environmentMock.Setup(e => e.GetFolderPath(specialFolder)).Returns(FolderPath);
    _fileMock.Setup(f => f.Exists(It.Is(shortcutPathMatch))).Returns(shortcutExists);
    var exists = _testeeService.Exists(specialFolder, out var shortcutPath);
    exists.Should().Be(shortcutExists);
    shortcutPath.Should().Match(shortcutPathMatch);
  }


  [Theory, MemberData(nameof(SpecialFoldersData))]
  internal void Create_ShortcutExists_NoCreateAction(Environment.SpecialFolder specialFolder)
  {
    const string ProductName = "Just Clipboard Manager";
    const string FolderPath = @"X:\SpecialFolder\Path";
    _infoServiceMock.Setup(i => i.ProductName).Returns(ProductName);
    _environmentMock.Setup(e => e.GetFolderPath(specialFolder)).Returns(FolderPath);
    _fileMock.Setup(f => f.Exists(It.IsAny<string>())).Returns(true);
    _testeeService.Create(specialFolder);
    _wshShellMock.VerifyNoOtherCalls();
  }


  [Theory, MemberData(nameof(SpecialFoldersData))]
  internal void Create_ShortcutDoesNotExist_ShortcutIsCreated(Environment.SpecialFolder specialFolder)
  {
    const string ProductName = "Just Clipboard Manager";
    const string FolderPath = @"X:\SpecialFolder\Path";
    const string ProcessPath = @"C:\Users\AppData\JCM.exe";
    var processDirectory = Path.GetDirectoryName(ProcessPath)!;
    _infoServiceMock.Setup(i => i.ProductName).Returns(ProductName);
    _environmentMock.Setup(e => e.GetFolderPath(specialFolder)).Returns(FolderPath);
    _fileMock.Setup(f => f.Exists(It.IsAny<string>())).Returns(false);
    var shortcutMock = new Mock<IWshShortcut>();
    _wshShellMock.Setup(shell => shell.CreateShortcut(It.IsAny<string>())).Returns(shortcutMock.Object);
    _environmentMock.Setup(e => e.ProcessPath).Returns(ProcessPath);
    _pathMock.Setup(p => p.GetDirectoryName(ProcessPath)).Returns(processDirectory);
    _testeeService.Create(specialFolder);
    shortcutMock.VerifySet(sh => sh.TargetPath = ProcessPath, Times.Once);
    shortcutMock.VerifySet(sh => sh.WorkingDirectory = processDirectory, Times.Once);
    shortcutMock.VerifySet(
      sh => sh.IconLocation = It.Is<string>(s => s.StartsWith(Path.Combine(processDirectory, "tray"))
                                              && s.EndsWith(".ico")),
      Times.Once
    );
    shortcutMock.Verify(sh => sh.Save(), Times.Once);
  }


  [Theory, MemberData(nameof(SpecialFoldersData))]
  internal void Delete_ShortcutDoesNotExist_NoDeleteAction(Environment.SpecialFolder specialFolder)
  {
    const string ProductName = "Just Clipboard Manager";
    const string FolderPath = @"X:\SpecialFolder\Path";
    _infoServiceMock.Setup(i => i.ProductName).Returns(ProductName);
    _environmentMock.Setup(e => e.GetFolderPath(specialFolder)).Returns(FolderPath);
    _fileMock.Setup(f => f.Exists(It.IsAny<string>())).Returns(false);
    _testeeService.Delete(specialFolder);
    _fileMock.Verify(f => f.Delete(It.IsAny<string>()), Times.Never);
  }


  [Theory, MemberData(nameof(SpecialFoldersData))]
  internal void Delete_ShortcutExists_ShortcutIsDeleted(Environment.SpecialFolder specialFolder)
  {
    const string ProductName = "Just Clipboard Manager";
    const string FolderPath = @"X:\SpecialFolder\Path";
    _infoServiceMock.Setup(i => i.ProductName).Returns(ProductName);
    _environmentMock.Setup(e => e.GetFolderPath(specialFolder)).Returns(FolderPath);
    _fileMock.Setup(f => f.Exists(It.IsAny<string>())).Returns(true);
    _testeeService.Delete(specialFolder);
    _fileMock.Verify(
      f => f.Delete(It.Is<string>(s => s.StartsWith(Path.Combine(FolderPath, ProductName))
                                    && s.EndsWith(".lnk"))),
      Times.Once
    );
  }
}
