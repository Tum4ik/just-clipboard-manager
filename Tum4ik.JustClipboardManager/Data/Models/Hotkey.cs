using CommunityToolkit.Mvvm.ComponentModel;
using Tum4ik.JustClipboardManager.PluginDevKit.Models;

namespace Tum4ik.JustClipboardManager.Data.Models;
internal partial class Hotkey : ObservableObject
{
  public Hotkey(string descriptionKey,
                KeyBindingDescriptor keyBindingDescriptor,
                Func<KeyBindingDescriptor, bool> registerAction)
  {
    DescriptionKey = descriptionKey;
    _keyBindingDescriptor = keyBindingDescriptor;
    RegisterAction = registerAction;
  }

  public string DescriptionKey { get; }
  [ObservableProperty] private KeyBindingDescriptor _keyBindingDescriptor;
  public Func<KeyBindingDescriptor, bool> RegisterAction { get; }
}
