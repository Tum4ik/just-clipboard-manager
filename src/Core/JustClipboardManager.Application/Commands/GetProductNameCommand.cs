using JustClipboardManager.Application.Common.Abstractions;
using JustClipboardManager.Application.Services;
using JustClipboardManager.Domain.Common;

namespace JustClipboardManager.Application.Commands;

internal class GetProductNameCommand : AppCommand<None, string>
{
  private readonly IProductInfoService _productInfoService;

  public GetProductNameCommand(IProductInfoService productInfoService)
  {
    _productInfoService = productInfoService;
  }

  protected override Task<string> ExecuteAsync(None parameter, CancellationToken cancellationToken = default)
  {
    return Task.FromResult(_productInfoService.GetProductName());
  }
}
