using System.Windows;
using System.Windows.Controls;
using Prism.Ioc;
using Tum4ik.JustClipboardManager.Data.Dto;
using Tum4ik.JustClipboardManager.PluginDevKit.Extensions;

namespace Tum4ik.JustClipboardManager.Controls;
internal class ClipTypeDataTemplateSelector : DataTemplateSelector
{
  public override DataTemplate? SelectTemplate(object item, DependencyObject container)
  {
    var clipDto = (ClipDto) item;
    var plugin = ContainerLocator.Container.ResolvePlugin(clipDto.PluginId);
    return plugin.RepresentationDataDataTemplate;
  }
}
