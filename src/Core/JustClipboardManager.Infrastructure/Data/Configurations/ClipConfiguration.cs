using JustClipboardManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JustClipboardManager.Infrastructure.Data.Configurations;

internal class ClipConfiguration : IEntityTypeConfiguration<Clip>
{
  public void Configure(EntityTypeBuilder<Clip> builder)
  {
    builder
      .ToTable(b => b.HasComment("Represents a single item in the clipboard history."))
      .Property(c => c.ClippedAt).HasDefaultValueSql("datetime('now', 'localtime')");
    builder
      .HasIndex(c => c.SearchLabel);
  }
}
