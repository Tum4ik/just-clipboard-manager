using System.Reflection;

namespace Tum4ik.JustClipboardManager.Services;
internal class InfoService : IInfoService
{
  private string? _productName;
  public string ProductName
  {
    get
    {
      return _productName ??= Assembly.GetEntryAssembly()?
        .GetCustomAttribute<AssemblyProductAttribute>()?
        .Product ?? "unknown product";
    }
  }


  private string? _informationalVersion;
  public string InformationalVersion
  {
    get
    {
      return _informationalVersion ??= Assembly.GetEntryAssembly()?
        .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
        .InformationalVersion ?? "unknown version";
    }
  }


  private Version? _version;
  public Version Version
  {
    get
    {
      if (_version is null)
      {
        var versionString = Assembly.GetEntryAssembly()?
          .GetCustomAttribute<AssemblyFileVersionAttribute>()?
          .Version;
        _version = Version.TryParse(versionString, out var version) ? version : new Version();
      }
      return _version;
    }
  }
}
