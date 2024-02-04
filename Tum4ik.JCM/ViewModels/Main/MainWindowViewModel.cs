using CommunityToolkit.Mvvm.ComponentModel;
using Tum4ik.JustClipboardManager.Services;

namespace Tum4ik.JustClipboardManager.ViewModels.Main;
internal sealed partial class MainWindowViewModel
{
  private readonly IInfoService _infoService;

  public MainWindowViewModel(IInfoService infoService)
  {
    _infoService = infoService;
  }


  private string? _title;
  public string Title => _title ??= _infoService.ProductName;
}
