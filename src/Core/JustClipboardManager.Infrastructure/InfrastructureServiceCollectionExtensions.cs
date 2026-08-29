using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JustClipboardManager.Infrastructure;

public static class InfrastructureServiceCollectionExtensions
{
  public static IServiceCollection AddInfrastructure(this IServiceCollection services)
  {
    services.AddSingleton(BuildConfiguration());

    return services;
  }


  private static IConfiguration BuildConfiguration()
  {
    var assembly = Assembly.GetExecutingAssembly();
    var name = assembly.GetName().Name;
    using var jsonStream = assembly.GetManifestResourceStream($"{name}.appsettings.json");
    var configuration = new ConfigurationBuilder()
      .AddJsonStream(jsonStream!)
      .AddJsonFile("appsettings.Development.json", optional: true)
      .Build();
    return configuration;
  }
}
