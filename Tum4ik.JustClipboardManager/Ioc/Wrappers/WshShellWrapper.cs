using System.Diagnostics.CodeAnalysis;
using IWshRuntimeLibrary;

namespace Tum4ik.JustClipboardManager.Ioc.Wrappers;

#pragma warning disable IDE1006 // Naming Styles

[ExcludeFromCodeCoverage]
internal sealed class WshShellWrapper : WshShell
{
  private readonly WshShell _instance = new();

  public int Run(string Command, ref object WindowStyle, ref object WaitOnReturn)
  {
    return _instance.Run(Command, ref WindowStyle, ref WaitOnReturn);
  }

  public int Popup(string Text, ref object SecondsToWait, ref object Title, ref object Type)
  {
    return _instance.Popup(Text, ref SecondsToWait, ref Title, ref Type);
  }

  public dynamic CreateShortcut(string PathLink)
  {
    return _instance.CreateShortcut(PathLink);
  }

  public string ExpandEnvironmentStrings(string Src)
  {
    return _instance.ExpandEnvironmentStrings(Src);
  }

  public dynamic RegRead(string Name)
  {
    return _instance.RegRead(Name);
  }

  public void RegWrite(string Name, ref object Value, ref object Type)
  {
    _instance.RegWrite(Name, ref Value, ref Type);
  }

  public void RegDelete(string Name)
  {
    _instance.RegDelete(Name);
  }

  public bool LogEvent(ref object Type, string Message, string Target = "")
  {
    return _instance.LogEvent(ref Type, Message, Target);
  }

  public bool AppActivate(ref object App, ref object Wait)
  {
    return _instance.AppActivate(ref App, ref Wait);
  }

  public void SendKeys(string Keys, ref object Wait)
  {
    _instance.SendKeys(Keys, ref Wait);
  }

  public WshExec Exec(string Command)
  {
    return _instance.Exec(Command);
  }

  public IWshCollection SpecialFolders => _instance.SpecialFolders;

  public string CurrentDirectory
  {
    get => _instance.CurrentDirectory;
    set => _instance.CurrentDirectory = value;
  }

  IWshEnvironment IWshShell3.get_Environment(ref object Type)
  {
    return _instance.Environment;
  }

  IWshEnvironment IWshShell2.get_Environment(ref object Type)
  {
    return _instance.Environment;
  }

  IWshEnvironment IWshShell.get_Environment(ref object Type)
  {
    return _instance.Environment;
  }
}
