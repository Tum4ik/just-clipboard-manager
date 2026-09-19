using JustClipboardManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace JustClipboardManager.Application.Common.Abstractions;

public abstract class AppDbContext : DbContext
{
  public AppDbContext(DbContextOptions options) : base(options)
  {
  }

  public DbSet<Clip> Clips => Set<Clip>();
  public DbSet<ClipDataObject> ClipDataObjects => Set<ClipDataObject>();
}
