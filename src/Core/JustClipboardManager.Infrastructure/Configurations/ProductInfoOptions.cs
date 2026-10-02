using System.ComponentModel.DataAnnotations;

namespace JustClipboardManager.Infrastructure.Configurations;

internal class ProductInfoOptions
{
  public const string ProductInfo = nameof(ProductInfo);

  [Required] public required string Name { get; init; }
  [Required] public required Version Version { get; init; }
}
