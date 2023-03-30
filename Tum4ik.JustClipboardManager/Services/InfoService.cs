using System.Reflection;
using Tum4ik.JustClipboardManager.Interfaces.Services;

namespace Tum4ik.JustClipboardManager.Services;
internal class InfoService : IInfoService
{
  public string ProductName
  {
    get
    {
      return Assembly.GetEntryAssembly()?
        .GetCustomAttribute<AssemblyProductAttribute>()?
        .Product ?? "unknown product";
    }
  }


  public string InformationalVersion
  {
    get
    {
      return Assembly.GetEntryAssembly()?
        .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
        .InformationalVersion ?? "unknown version";
    }
  }


  public Version Version
  {
    get
    {
      var versionString = Assembly.GetEntryAssembly()?
        .GetCustomAttribute<AssemblyFileVersionAttribute>()?
        .Version;
      return Version.TryParse(versionString, out var version) ? version : new Version();
    }
  }
}
