using System.IO;
using IWshRuntimeLibrary;
using Tum4ik.JustClipboardManager.Ioc.Wrappers;
using IFile = Tum4ik.JustClipboardManager.Ioc.Wrappers.IFile;

namespace Tum4ik.JustClipboardManager.Services;

internal class ShortcutService : IShortcutService
{
  private readonly IInfoService _infoService;
  private readonly WshShell _wshShell;
  private readonly IFile _file;
  private readonly IPath _path;
  private readonly IEnvironment _environment;

  public ShortcutService(IInfoService infoService,
                         WshShell wshShell,
                         IFile file,
                         IPath path,
                         IEnvironment environment)
  {
    _infoService = infoService;
    _wshShell = wshShell;
    _file = file;
    _path = path;
    _environment = environment;
  }


  public bool Exists(Environment.SpecialFolder specialFolder, out string path)
  {
    path = GetShortcutPath(specialFolder);
    return _file.Exists(path);
  }


  public void Create(Environment.SpecialFolder specialFolder)
  {
    if (Exists(specialFolder, out var shortcutPath))
    {
      return;
    }

    IWshShortcut shortcut = _wshShell.CreateShortcut(shortcutPath);
    var processPath = _environment.ProcessPath;
    var directory = _path.GetDirectoryName(processPath)!;
#if DEBUG
    var iconFileName = "tray-dev.ico";
#else
    var iconFileName = "tray.ico";
#endif
    shortcut.TargetPath = processPath;
    shortcut.WorkingDirectory = directory;
    shortcut.IconLocation = Path.Combine(directory, iconFileName);
    shortcut.Save();
  }


  public void Delete(Environment.SpecialFolder specialFolder)
  {
    if (Exists(specialFolder, out var shortcutPath))
    {
      _file.Delete(shortcutPath);
    }
  }


  private string GetShortcutPath(Environment.SpecialFolder folder)
  {
    var productName = _infoService.ProductName;
#if DEBUG
    productName += " (Dev)";
#endif
    return Path.Combine(_environment.GetFolderPath(folder), $"{productName}.lnk");
  }
}
