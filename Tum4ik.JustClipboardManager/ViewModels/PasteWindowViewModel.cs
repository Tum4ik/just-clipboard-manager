using System;
using CommunityToolkit.Mvvm.Input;
using PubSub;
using Tum4ik.JustClipboardManager.EventPayloads;
using Tum4ik.JustClipboardManager.Mvvm;

namespace Tum4ik.JustClipboardManager.ViewModels;

internal partial class PasteWindowViewModel : IWindowAware
{
  private readonly IPublisher _publisher;

  public PasteWindowViewModel(IPublisher publisher)
  {
    _publisher = publisher;
  }


  private Action? _hideWindow;
  

  public void WindowActions(Action hide)
  {
    _hideWindow = hide;
  }


  [ICommand]
  private void PasteData()
  {
    _hideWindow?.Invoke();
    _publisher.Publish(new PasteWindowResult("test paste"));
  }
}
