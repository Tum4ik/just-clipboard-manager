using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using Tum4ik.JustClipboardManager.Icons;

namespace Tum4ik.JustClipboardManager.ViewModels.Main.Settings;
internal partial class SettingsInterfaceViewModel : ObservableObject
{
  public SettingsInterfaceViewModel()
  {
    SelectedLanguage = Languages.Skip(1).First();
  }


  public IEnumerable<Language> Languages { get; } = new[]
  {
    new Language(new("en-US"), SvgIconType.USA),
    new Language(new("uk-UA"), SvgIconType.Ukraine)
  };


  [ObservableProperty] private Language? _selectedLanguage;
}


internal class Language
{
  public Language(CultureInfo culture, SvgIconType icon)
  {
    Culture = culture;
    Icon = icon;
  }


  public CultureInfo Culture { get; }
  public SvgIconType Icon { get; }
}
