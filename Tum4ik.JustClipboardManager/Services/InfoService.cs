using System.Reflection;

namespace Tum4ik.JustClipboardManager.Services;
internal class InfoService : IInfoService
{
  public string GetInformationalVersion()
  {
    return Assembly.GetEntryAssembly()?
      .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
      .InformationalVersion ?? "unknown";
  }


  public Version GetVersion()
  {
    var versionString = Assembly.GetEntryAssembly()?
      .GetCustomAttribute<AssemblyFileVersionAttribute>()?
      .Version;
    return Version.TryParse(versionString, out var version) ? version : new Version();
  }
}
