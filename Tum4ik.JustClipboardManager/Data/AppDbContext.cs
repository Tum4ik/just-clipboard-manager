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
}
