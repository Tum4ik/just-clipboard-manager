using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Tum4ik.JustClipboardManager.Extensions;
using Tum4ik.JustClipboardManager.Helpers;

namespace Tum4ik.JustClipboardManager.Data;

/// <summary>
/// The class is used to produce EF migrations.
/// </summary>
internal class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
  public AppDbContext CreateDbContext(string[] args)
  {
    var (configuration, _) = ConfigurationHelper.CreateConfiguration(args);
    var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>().Configure(configuration);
    return new(optionsBuilder.Options);
  }
}
