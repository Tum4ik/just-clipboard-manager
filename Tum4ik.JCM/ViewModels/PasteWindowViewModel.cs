using System.Collections.ObjectModel;
using Tum4ik.JustClipboardManager.Controls;
using Tum4ik.JustClipboardManager.Data.Dto;

namespace Tum4ik.JustClipboardManager.ViewModels;
internal sealed class PasteWindowViewModel
{
  public ClipTypeDataTemplateSelector ClipTypeDataTemplateSelector { get; }

  public PasteWindowViewModel(ClipTypeDataTemplateSelector clipTypeDataTemplateSelector)
  {
    ClipTypeDataTemplateSelector = clipTypeDataTemplateSelector;
  }


  public ObservableCollection<ClipDto> Clips { get; } = new()
  {
    new() {Id=1, PluginId="D930D2CD-3FD9-4012-A363-120676E22AFA", RepresentationData="Test"},
    new() {Id=2, PluginId="D930D2CD-3FD9-4012-A363-120676E22AFA", RepresentationData="Test1"}
  };
}
