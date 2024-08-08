using System.IO;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Tum4ik.JustClipboardManager.Extensions;
internal static class DbContextOptionsBuilderExtensions
{
  public static DbContextOptionsBuilder Configure(this DbContextOptionsBuilder builder,
                                                  IConfiguration configuration)
  {
    var dbFilePath = GetDatabaseFilePath(configuration);
    return builder
      .UseSqlite($"Data Source={dbFilePath}")
      .UseLazyLoadingProxies();
  }


  public static DbContextOptionsBuilder<T> Configure<T>(this DbContextOptionsBuilder<T> builder,
                                                        IConfiguration configuration)
    where T : DbContext
  {
    return (DbContextOptionsBuilder<T>) ((DbContextOptionsBuilder) builder).Configure(configuration);
  }


  private static string GetDatabaseFilePath(IConfiguration configuration)
  {
    var companyName = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyCompanyAttribute>()!.Company;
    var dbFileDir = Path.Combine(
      Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
      companyName,
      Assembly.GetExecutingAssembly().GetName().Name!,
      "Database"
    );

    var dbName = configuration["Database:Name"]!;

    Directory.CreateDirectory(dbFileDir);
    var dbFilePath = Path.Combine(dbFileDir, dbName);
    return dbFilePath;
  }
}
