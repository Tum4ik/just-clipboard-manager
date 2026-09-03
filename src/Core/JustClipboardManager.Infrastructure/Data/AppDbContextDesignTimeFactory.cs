using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace JustClipboardManager.Infrastructure.Data;

internal sealed class AppDbContextDesignTimeFactory : IDesignTimeDbContextFactory<AppDbContext>
{
  public AppDbContext CreateDbContext(string[] args)
  {
    var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
    AppDbContext.ConfigureOptions(optionsBuilder, "Data Source=design-time.db");
    return new AppDbContext(optionsBuilder.Options);
  }
}
