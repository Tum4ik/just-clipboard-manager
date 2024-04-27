using System.IO;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Tum4ik.JustClipboardManager.Data.Models;

namespace Tum4ik.JustClipboardManager.Data;
internal class AppDbContext : DbContext
{
  public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
  {
  }


  public DbSet<Clip> Clips { get; set; }
  public DbSet<PinnedClip> PinnedClips { get; set; }
  public DbSet<FormattedDataObject> FormattedDataObjects { get; set; }

  public DbSet<Plugin> Plugins { get; set; }
  public DbSet<PluginFile> PluginFiles { get; set; }


  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder
      .Entity<Clip>()
      .Property(c => c.ClippedAt)
      .HasDefaultValueSql("datetime('now', 'localtime')");
  }


  public static void Configure(DbContextOptionsBuilder builder)
  {
    builder
      .UseSqlite($"Data Source={DbFilePath}")
      .UseLazyLoadingProxies();
  }


  private static string? _dbFilePath;
  public static string DbFilePath => _dbFilePath ??= GetDbFilePath();


  private static string GetDbFilePath()
  {
    var companyName = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyCompanyAttribute>()!.Company;
    var dbFileDir = Path.Combine(
      Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
      companyName,
      Assembly.GetExecutingAssembly().GetName().Name!,
      "Database"
    );


#if DEBUG
    const string DbName = "clips_debug.db";
#else
    const string DbName = "clips.db";
#endif
    Directory.CreateDirectory(dbFileDir);
    var dbFilePath = Path.Combine(dbFileDir, DbName);
    return dbFilePath;
  }
}
