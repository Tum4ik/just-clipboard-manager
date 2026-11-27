using JustClipboardManager.Application.Common.Abstractions;
using JustClipboardManager.Application.IPC.Commands;
using JustClipboardManager.Application.IPC.Common;
using JustClipboardManager.Application.Services;

namespace JustClipboardManager.Application.Commands;

internal class GetProductNameCommand : AppCommand<Empty, GetProductNameResult>
{
  private readonly IProductInfoService _productInfoService;

  public GetProductNameCommand(IProductInfoService productInfoService)
  {
    _productInfoService = productInfoService;
  }

  protected override Task<GetProductNameResult> ExecuteAsync(Empty parameter, CancellationToken cancellationToken = default)
  {
    var productName = _productInfoService.GetProductName();
    return Task.FromResult<GetProductNameResult>(new() { ProductName = productName });
  }
}
