using CommunityToolkit.Mvvm.ComponentModel;

namespace Tum4ik.JustClipboardManager.Data.Dto;

[INotifyPropertyChanged]
internal partial class InstalledPluginInfoDto : PluginInfoDto
{
  [ObservableProperty]
  private bool _isEnabled;
}
