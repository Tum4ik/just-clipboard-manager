using CommunityToolkit.Mvvm.ComponentModel;

namespace Tum4ik.JustClipboardManager.Data.Dto;

[INotifyPropertyChanged]
internal partial class InstalledPluginInfoDto : PluginInfoDto
{
  [ObservableProperty, NotifyPropertyChangedFor(nameof(IsInstalledAndEnabled))]
  private bool _isEnabled;
  [ObservableProperty, NotifyPropertyChangedFor(nameof(IsInstalledAndEnabled))]
  private bool _isInstalled;

  public bool IsInstalledAndEnabled => IsInstalled && IsEnabled;
}
