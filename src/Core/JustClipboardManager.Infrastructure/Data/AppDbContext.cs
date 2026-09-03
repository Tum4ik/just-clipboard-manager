using System.Reflection;
using JustClipboardManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace JustClipboardManager.Infrastructure.Data;

internal sealed class AppDbContext : DbContext
{
  public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
  {
  }

  internal static void ConfigureOptions(DbContextOptionsBuilder options, string connectionString)
  {
    options.UseSqlite(connectionString);
    options.UseLazyLoadingProxies();
  }

  public DbSet<Clip> Clips => Set<Clip>();
  public DbSet<ClipDataObject> ClipDataObjects => Set<ClipDataObject>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
  }
}
