using CommunityToolkit.Mvvm.Input;
using Tum4ik.JustClipboardManager.Services;

namespace Tum4ik.JustClipboardManager.ViewModels;

internal partial class PasteWindowViewModel
{
  private readonly ITargetWindowService _targetWindowService;

  public PasteWindowViewModel(ITargetWindowService targetWindowService)
  {
    _targetWindowService = targetWindowService;
  }


  [ICommand]
  private void PasteData()
  {
    _targetWindowService.PasteData("some test data");
  }
}
