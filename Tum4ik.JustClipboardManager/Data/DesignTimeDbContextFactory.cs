using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Tum4ik.JustClipboardManager.Data;

/// <summary>
/// The class is used to produce EF migrations.
/// </summary>
internal class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
  public AppDbContext CreateDbContext(string[] args)
  {
    var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
    AppDbContext.Configure(optionsBuilder);
    return new(optionsBuilder.Options);
  }
}
