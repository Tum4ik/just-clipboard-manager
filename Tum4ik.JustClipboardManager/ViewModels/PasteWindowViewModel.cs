using System;
using CommunityToolkit.Mvvm.Input;
using Tum4ik.EventAggregator;
using Tum4ik.JustClipboardManager.Events;
using Tum4ik.JustClipboardManager.Mvvm;

namespace Tum4ik.JustClipboardManager.ViewModels;

internal partial class PasteWindowViewModel : IWindowAware
{
  private readonly IEventPublisher _eventPublisher;

  public PasteWindowViewModel(IEventPublisher eventPublisher)
  {
    _eventPublisher = eventPublisher;
  }


  private Action? _hideWindow;
  

  public void WindowActions(Action hide)
  {
    _hideWindow = hide;
  }


  [ICommand]
  private void PasteData(string data)
  {
    _hideWindow?.Invoke();
    _eventPublisher.Publish(new PasteWindowResultEvent(data));
  }
}
