using CommunityToolkit.Mvvm.Input;
using Tum4ik.EventAggregator;
using Tum4ik.JustClipboardManager.Events;
using Tum4ik.JustClipboardManager.Mvvm;

namespace Tum4ik.JustClipboardManager.ViewModels;

internal partial class PasteWindowViewModel : IWindowAware
{
  private readonly IEventAggregator _eventAggregator;

  public PasteWindowViewModel(IEventAggregator eventAggregator)
  {
    _eventAggregator = eventAggregator;
  }


  private Action? _hideWindow;
  

  public void WindowActions(Action hide)
  {
    _hideWindow = hide;
  }


  [RelayCommand]
  private void PasteData(string data)
  {
    _hideWindow?.Invoke();
    _eventAggregator.GetEvent<PasteWindowResultEvent>().Publish(data);
  }
}
