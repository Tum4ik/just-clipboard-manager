using System.ComponentModel.DataAnnotations;

namespace JustClipboardManager.Infrastructure.Configurations;

internal class DatabaseOptions
{
  public const string Database = nameof(Database);

  [Required] public required string Name { get; init; }
}
