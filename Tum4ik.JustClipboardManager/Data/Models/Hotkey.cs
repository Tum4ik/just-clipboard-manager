using CommunityToolkit.Mvvm.ComponentModel;

namespace Tum4ik.JustClipboardManager.Data.Models;
internal partial class Hotkey : ObservableObject
{
  public string? Description { get; init; }
  [ObservableProperty] private KeyBindingDescriptor? _keyBindingDescriptor;
  public Func<KeyBindingDescriptor, bool>? RegisterAction { get; init; }
  public Action<KeyBindingDescriptor>? SettingsSaveAction { get; init; }
}
