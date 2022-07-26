using System;
using CommunityToolkit.Mvvm.Input;
using Tum4ik.JustClipboardManager.Mvvm;
using Tum4ik.JustClipboardManager.Services;

namespace Tum4ik.JustClipboardManager.ViewModels;

internal partial class PasteWindowViewModel : IWindowAware
{
  public PasteWindowResult? PasteResult { get; private set; }
  

  private Action? _hideWindow;
  

  public void WindowActions(Action hide)
  {
    _hideWindow = hide;
  }


  [ICommand]
  private void PasteData()
  {
    PasteResult = null;
    // todo: get data
    PasteResult = new("result");
    _hideWindow?.Invoke();
  }
}
