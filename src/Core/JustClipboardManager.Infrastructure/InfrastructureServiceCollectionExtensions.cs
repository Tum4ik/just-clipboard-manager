using System.Reflection;
using JustClipboardManager.Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using JustClipboardManager.Infrastructure.Extensions;
using JustClipboardManager.Infrastructure.Configurations;
using Microsoft.Extensions.Options;
using JustClipboardManager.Infrastructure.Services;
using JustClipboardManager.Application.Services;
using Microsoft.EntityFrameworkCore;
using JustClipboardManager.Application.Common.Abstractions;

namespace JustClipboardManager.Infrastructure;

public static class InfrastructureServiceCollectionExtensions
{
  public static IServiceCollection AddInfrastructure(this IServiceCollection services)
  {
    var configuration = BuildConfiguration();
    services.AddSingleton(configuration);
    services.ConfigureWithValidateOnStart<DatabaseOptions>(configuration.GetSection(DatabaseOptions.Database));
    services.ConfigureWithValidateOnStart<ProductInfoOptions>(configuration.GetSection(ProductInfoOptions.ProductInfo));
    services.AddPooledDbContextFactory<InfraDbContext>((provider, options) =>
    {
      var databaseOptions = provider.GetRequiredService<IOptions<DatabaseOptions>>().Value;
      var dbFilePath = BuildDbFilePath(databaseOptions.Name);
      InfraDbContext.ConfigureOptions(options, $"Data Source={dbFilePath}");
    });
    services.AddSingleton<IDbContextFactory<AppDbContext>>(
      provider => new AppDbContextFactory(provider.GetRequiredService<IDbContextFactory<InfraDbContext>>())
    );

    services.AddTransient<IDatabaseMigrator, DatabaseMigrator>();
    services.AddSingleton<IProductInfoService, ProductInfoService>();

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


  private static string BuildDbFilePath(string dbFileName)
  {
    var dir = Path.Combine(
      Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
      "JustClipboardManager" // todo: replace product name from other place (for ex. appsettings.json)
    );
    Directory.CreateDirectory(dir);
    return Path.Combine(dir, dbFileName);
  }


  private sealed class AppDbContextFactory(IDbContextFactory<InfraDbContext> infraFactory) : IDbContextFactory<AppDbContext>
  {
    public AppDbContext CreateDbContext() => infraFactory.CreateDbContext();

    public async Task<AppDbContext> CreateDbContextAsync(CancellationToken cancellationToken = default)
      => await infraFactory.CreateDbContextAsync(cancellationToken);
  }
}
