using System.Linq.Expressions;
using IWshRuntimeLibrary;
using Tum4ik.JustClipboardManager.Ioc.Wrappers;
using Tum4ik.JustClipboardManager.Services;
using IFile = Tum4ik.JustClipboardManager.Ioc.Wrappers.IFile;

namespace Tum4ik.JustClipboardManager.UnitTests.Services;
public class ShortcutServiceTests
{
  private readonly IInfoService _infoService = Substitute.For<IInfoService>();
  private readonly WshShell _wshShell = Substitute.For<WshShell>();
  private readonly IFile _file = Substitute.For<IFile>();
  private readonly IPath _path = Substitute.For<IPath>();
  private readonly IEnvironment _environment = Substitute.For<IEnvironment>();
  private readonly ShortcutService _testeeService;

  public ShortcutServiceTests()
  {
    _testeeService = new(
      _infoService,
      _wshShell,
      _file,
      _path,
      _environment
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
    _infoService.ProductName.Returns(ProductName);
    _environment.GetFolderPath(specialFolder).Returns(FolderPath);
    _file.Exists(Arg.Is<string>(s => shortcutPathMatch.Compile()(s))).Returns(shortcutExists);
    var exists = _testeeService.Exists(specialFolder, out var shortcutPath);
    exists.Should().Be(shortcutExists);
    shortcutPath.Should().Match(shortcutPathMatch);
  }


  [Theory, MemberData(nameof(SpecialFoldersData))]
  internal void Create_ShortcutExists_NoCreateAction(Environment.SpecialFolder specialFolder)
  {
    const string ProductName = "Just Clipboard Manager";
    const string FolderPath = @"X:\SpecialFolder\Path";
    _infoService.ProductName.Returns(ProductName);
    _environment.GetFolderPath(specialFolder).Returns(FolderPath);
    _file.Exists(Arg.Any<string>()).Returns(true);
    _testeeService.Create(specialFolder);
    _wshShell.ReceivedCalls().Any().Should().BeFalse();
  }


  [Theory, MemberData(nameof(SpecialFoldersData))]
  internal void Create_ShortcutDoesNotExist_ShortcutIsCreated(Environment.SpecialFolder specialFolder)
  {
    const string ProductName = "Just Clipboard Manager";
    const string FolderPath = @"X:\SpecialFolder\Path";
    const string ProcessPath = @"C:\Users\AppData\JCM.exe";
    var processDirectory = Path.GetDirectoryName(ProcessPath)!;
    _infoService.ProductName.Returns(ProductName);
    _environment.GetFolderPath(specialFolder).Returns(FolderPath);
    _file.Exists(Arg.Any<string>()).Returns(false);
    var shortcut = Substitute.For<IWshShortcut>();
    _wshShell.CreateShortcut(Arg.Any<string>()).Returns(shortcut); // <-- Microsoft.CSharp.RuntimeBinder.RuntimeBinderException : Cannot perform runtime binding on a null reference
    _environment.ProcessPath.Returns(ProcessPath);
    _path.GetDirectoryName(ProcessPath).Returns(processDirectory);
    _testeeService.Create(specialFolder);
    shortcut.Received(1).TargetPath = ProcessPath;
    shortcut.Received(1).WorkingDirectory = processDirectory;
    shortcut.Received(1).IconLocation = Arg.Is<string>(s => s.StartsWith(Path.Combine(processDirectory, "tray"))
                                                            && s.EndsWith(".ico"));
    shortcut.Received(1).Save();
  }


  [Theory, MemberData(nameof(SpecialFoldersData))]
  internal void Delete_ShortcutDoesNotExist_NoDeleteAction(Environment.SpecialFolder specialFolder)
  {
    const string ProductName = "Just Clipboard Manager";
    const string FolderPath = @"X:\SpecialFolder\Path";
    _infoService.ProductName.Returns(ProductName);
    _environment.GetFolderPath(specialFolder).Returns(FolderPath);
    _file.Exists(Arg.Any<string>()).Returns(false);
    _testeeService.Delete(specialFolder);
    _file.DidNotReceiveWithAnyArgs().Delete(default!);
  }


  [Theory, MemberData(nameof(SpecialFoldersData))]
  internal void Delete_ShortcutExists_ShortcutIsDeleted(Environment.SpecialFolder specialFolder)
  {
    const string ProductName = "Just Clipboard Manager";
    const string FolderPath = @"X:\SpecialFolder\Path";
    _infoService.ProductName.Returns(ProductName);
    _environment.GetFolderPath(specialFolder).Returns(FolderPath);
    _file.Exists(Arg.Any<string>()).Returns(true);
    _testeeService.Delete(specialFolder);
    _file.Received(1).Delete(Arg.Is<string>(s => s.StartsWith(Path.Combine(FolderPath, ProductName))
                                                 && s.EndsWith(".lnk")));
  }
}
