using Microsoft.EntityFrameworkCore;
using Tum4ik.JustClipboardManager.Data.Models;

namespace Tum4ik.JustClipboardManager.Data;
internal class AppDbContext : DbContext
{
  public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
  {
    // TODO: use migrations
    Database.EnsureCreated();
  }


  public DbSet<Clip> Clips { get; set; } = null!;
  public DbSet<FormattedDataObject> FormattedDataObjects { get; set; } = null!;


  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<Clip>()
      .Property(c => c.ClippedAt)
      .HasDefaultValueSql("datetime('now', 'localtime')");
  }
}
