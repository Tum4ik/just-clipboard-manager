using System.ComponentModel.DataAnnotations;

namespace JustClipboardManager.Infrastructure.Configurations;

internal sealed class DatabaseOptions
{
  public const string Database = nameof(Database);

  [Required] public required string Name { get; init; }
}
