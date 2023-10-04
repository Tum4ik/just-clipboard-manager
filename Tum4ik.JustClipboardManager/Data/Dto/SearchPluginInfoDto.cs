using CommunityToolkit.Mvvm.ComponentModel;

namespace Tum4ik.JustClipboardManager.Data.Dto;

[INotifyPropertyChanged]
internal partial class SearchPluginInfoDto : PluginInfoDto
{
  public required Uri DownloadLink { get; init; }
  [ObservableProperty] private bool _isInstalled;
}
