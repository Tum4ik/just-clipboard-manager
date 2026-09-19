using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace JustClipboardManager.Infrastructure.Data;

internal sealed class AppDbContextDesignTimeFactory : IDesignTimeDbContextFactory<InfraDbContext>
{
  public InfraDbContext CreateDbContext(string[] args)
  {
    var optionsBuilder = new DbContextOptionsBuilder<InfraDbContext>();
    InfraDbContext.ConfigureOptions(optionsBuilder, "Data Source=design-time.db");
    return new InfraDbContext(optionsBuilder.Options);
  }
}
