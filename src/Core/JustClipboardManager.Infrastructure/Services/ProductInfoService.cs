using JustClipboardManager.Application.Services;
using JustClipboardManager.Infrastructure.Configurations;
using Microsoft.Extensions.Options;

namespace JustClipboardManager.Infrastructure.Services;

internal class ProductInfoService : IProductInfoService
{
  private readonly ProductInfoOptions _productInfoOptions;

  public ProductInfoService(IOptions<ProductInfoOptions> productInfoOptions)
  {
    _productInfoOptions = productInfoOptions.Value;
  }

  public string GetProductName()
  {
    return _productInfoOptions.Name;
  }

  public Version GetProductVersion()
  {
    return _productInfoOptions.Version;
  }
}
