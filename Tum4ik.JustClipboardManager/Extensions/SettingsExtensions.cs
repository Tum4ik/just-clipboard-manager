using System.Configuration;

namespace Tum4ik.JustClipboardManager.Extensions;
internal static class SettingsExtensions
{
  public static T Get<T>(this SettingsBase settings, Guid key, T defaultValue = default!)
  {
    var keyStr = $"{typeof(T)}_{key}";
    settings.EnsureDynamicProperty<T>(keyStr);
    try
    {
      return (T) settings[keyStr];
    }
    catch (NotSupportedException)
    {
      // means the setting is touched first time ever
      settings[keyStr] = defaultValue;
      settings.Save();
      return defaultValue;
    }
  }


  public static void Save<T>(this SettingsBase settings, Guid key, T value)
  {
    var keyStr = $"{typeof(T)}_{key}";
    settings.EnsureDynamicProperty<T>(keyStr);
    settings[keyStr] = value;
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
