using CommunityToolkit.Mvvm.Input;
using Prism.Services.Dialogs;

namespace Tum4ik.JustClipboardManager.Services.Dialogs;

internal interface ISimpleDialogAware : IDialogAware
{
  IRelayCommand? CancelButtonPressedCommand { get; }
  IRelayCommand? AcceptButtonPressedCommand { get; }
  string? CancelButtonText { get; }
  string? AcceptButtonText { get; }
}
