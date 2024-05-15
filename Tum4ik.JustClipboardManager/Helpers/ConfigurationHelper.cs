using System.Reflection;
using Microsoft.Extensions.Configuration;
using Tum4ik.JustClipboardManager.Services;

namespace Tum4ik.JustClipboardManager.Helpers;
internal static class ConfigurationHelper
{
  public static (IConfiguration, AppEnvironment) CreateConfiguration(string[] args)
  {
    var assembly = Assembly.GetExecutingAssembly();
    var appsettingsResourceName = assembly.GetManifestResourceNames()
      .Single(n => n.EndsWith("appsettings.json", StringComparison.OrdinalIgnoreCase));
    // Don't use "using" keyword for appsettingsStream here - it will break the settings reading process.
    // The stream will be disposed by StreamReader internally anyway.
    var appsettingsStream = assembly.GetManifestResourceStream(appsettingsResourceName);
    var configurationBuilder = new ConfigurationBuilder()
      .SetBasePath(AppContext.BaseDirectory)
      .AddJsonStream(appsettingsStream!);

    var appEnvironment = AppEnvironment.Production;
    if (Array.Find(args, a => a.Equals("--development", StringComparison.OrdinalIgnoreCase)) is not null)
    {
      var appsettingsDevelopmentResourceName = assembly.GetManifestResourceNames()
        .SingleOrDefault(n => n.EndsWith("appsettings.development.json", StringComparison.OrdinalIgnoreCase));
      if (appsettingsDevelopmentResourceName is not null)
      {
        var appsettingsDevelopmentStream = assembly.GetManifestResourceStream(appsettingsDevelopmentResourceName);
        configurationBuilder.AddUserSecrets(Assembly.GetExecutingAssembly());
        configurationBuilder.AddJsonStream(appsettingsDevelopmentStream!);
        appEnvironment = AppEnvironment.Development;
      }
    }
    else if (Array.Find(args, a => a.Equals("--uitest", StringComparison.OrdinalIgnoreCase)) is not null)
    {
      var appsettingsUiTestResourceName = assembly.GetManifestResourceNames()
        .SingleOrDefault(n => n.EndsWith("appsettings.uitest.json", StringComparison.OrdinalIgnoreCase));
      if (appsettingsUiTestResourceName is not null)
      {
        var appsettingsUiTestStream = assembly.GetManifestResourceStream(appsettingsUiTestResourceName);
        configurationBuilder.AddJsonStream(appsettingsUiTestStream!);
        appEnvironment = AppEnvironment.UiTest;
      }
    }

    return (configurationBuilder.Build(), appEnvironment);
  }
}
