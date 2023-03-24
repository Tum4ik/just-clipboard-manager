using CommunityToolkit.Mvvm.ComponentModel;

namespace Tum4ik.JustClipboardManager.Data.Models;
internal partial class Hotkey : ObservableObject
{
  public string? Description { get; init; }
  [ObservableProperty] private KeybindDescriptor? _keybindDescriptor;
}
