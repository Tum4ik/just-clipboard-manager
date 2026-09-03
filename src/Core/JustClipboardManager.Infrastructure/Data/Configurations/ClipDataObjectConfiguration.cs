using JustClipboardManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JustClipboardManager.Infrastructure.Data.Configurations;

internal class ClipDataObjectConfiguration : IEntityTypeConfiguration<ClipDataObject>
{
  public void Configure(EntityTypeBuilder<ClipDataObject> builder)
  {
    builder
      .ToTable(b => b.HasComment("Contains a clipboard item data for a specific format."));
  }
}
