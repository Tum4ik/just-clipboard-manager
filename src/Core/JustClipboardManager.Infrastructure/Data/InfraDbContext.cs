using System.Reflection;
using JustClipboardManager.Application.Common.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace JustClipboardManager.Infrastructure.Data;

internal sealed class InfraDbContext : AppDbContext
{
  public InfraDbContext(DbContextOptions<InfraDbContext> options) : base(options)
  {
  }

  internal static void ConfigureOptions(DbContextOptionsBuilder options, string connectionString)
  {
    options.UseSqlite(connectionString);
    options.UseLazyLoadingProxies();
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
  }
}
