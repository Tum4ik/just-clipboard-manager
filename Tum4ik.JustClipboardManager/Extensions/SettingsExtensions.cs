using System.Configuration;

namespace Tum4ik.JustClipboardManager.Extensions;
internal static class SettingsExtensions
{
  public static T Get<T>(this SettingsBase settings, string key, T defaultValue = default!)
  {
    key = $"{typeof(T)}_{key}";
    settings.EnsureDynamicProperty<T>(key);
    try
    {
      return (T) settings[key];
    }
    catch (NotSupportedException)
    {
      // means the setting is touched first time ever
      settings[key] = defaultValue;
      settings.Save();
      return defaultValue;
    }
  }


  public static void Save<T>(this SettingsBase settings, string key, T value)
  {
    key = $"{typeof(T)}_{key}";
    settings.EnsureDynamicProperty<T>(key);
    settings[key] = value;
    settings.Save();
  }


  private static void EnsureDynamicProperty<T>(this SettingsBase settings,
                                               string propertyName)
  {
    if (settings.Properties[propertyName] is not null)
    {
      return;
    }

    var settingsProperty = new SettingsProperty(propertyName)
    {
      Provider = settings.Providers[nameof(LocalFileSettingsProvider)],
      Attributes =
      {
        { typeof(UserScopedSettingAttribute), new UserScopedSettingAttribute() }
      },
      PropertyType = typeof(T),
      DefaultValue = default(T)
    };
    settings.Properties.Add(settingsProperty);
  }
}
