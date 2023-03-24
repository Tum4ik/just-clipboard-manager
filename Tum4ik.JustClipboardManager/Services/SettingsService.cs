using System.Globalization;
using System.Text.Json;
using System.Windows.Input;
using Tum4ik.JustClipboardManager.Data.Models;
using Tum4ik.JustClipboardManager.Properties;

namespace Tum4ik.JustClipboardManager.Services;

internal class SettingsService : ISettingsService
{
  private CultureInfo? _language;
  public CultureInfo Language
  {
    get
    {
      if (_language is not null)
      {
        return _language;
      }

      var defaultCulture = CultureInfo.GetCultureInfo("en-US");
      if (SettingsInterface.Default.Language is null)
      {
        _language = defaultCulture;
        return _language;
      }

      try
      {
        _language = CultureInfo.GetCultureInfo(SettingsInterface.Default.Language);
      }
      catch (CultureNotFoundException)
      {
        _language = defaultCulture;
      }

      return _language;
    }
    set
    {
      SettingsInterface.Default.Language = value.Name;
      SettingsInterface.Default.Save();
      _language = value;
    }
  }


  private string? _theme;
  public string Theme
  {
    get => _theme ??= (SettingsInterface.Default.Theme ?? string.Empty);
    set
    {
      SettingsInterface.Default.Theme = value;
      SettingsInterface.Default.Save();
      _theme = value;
    }
  }


  private KeybindDescriptor? _keybindDescriptor;
  public KeybindDescriptor HotkeyShowPasteWindow
  {
    get
    {
      if (_keybindDescriptor is not null)
      {
        return _keybindDescriptor;
      }

      var defaultKeybind = new KeybindDescriptor(ModifierKeys.Control | ModifierKeys.Shift, Key.V);
      if (string.IsNullOrEmpty(SettingsHotkeys.Default.ShowPasteWindow))
      {
        _keybindDescriptor = defaultKeybind;
        return _keybindDescriptor;
      }

      try
      {
        _keybindDescriptor = JsonSerializer.Deserialize<KeybindDescriptor>(SettingsHotkeys.Default.ShowPasteWindow);
      }
      catch (JsonException)
      {
        _keybindDescriptor = defaultKeybind;
      }

      return _keybindDescriptor ??= defaultKeybind;
    }
    set
    {
      SettingsHotkeys.Default.ShowPasteWindow = JsonSerializer.Serialize(value);
      SettingsHotkeys.Default.Save();
      _keybindDescriptor = value;
    }
  }
}
