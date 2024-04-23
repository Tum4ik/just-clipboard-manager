using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace Tum4ik.JustClipboardManager.Helpers;
internal static class ConfigurationHelper
{
  public static IConfiguration CreateConfiguration()
  {
    var assembly = Assembly.GetExecutingAssembly();
    var appsettingsResourceName = assembly.GetManifestResourceNames()
      .Single(n => n.EndsWith("appsettings.json", StringComparison.OrdinalIgnoreCase));
    // Don't use "using" keyword for appsettingsStream here - it will break the settings reading process.
    // The stream will be disposed by StreamReader internally anyway.
    var appsettingsStream = assembly.GetManifestResourceStream(appsettingsResourceName);
    return new ConfigurationBuilder()
      .SetBasePath(AppContext.BaseDirectory)
      .AddJsonStream(appsettingsStream!)
#if DEBUG
      .AddUserSecrets(Assembly.GetExecutingAssembly())
#endif
      .Build();
  }
}
