using System.Windows;
using System.Windows.Controls;
using Prism.Ioc;
using Tum4ik.JustClipboardManager.Data.Dto;
using Tum4ik.JustClipboardManager.Services.Plugins;

namespace Tum4ik.JustClipboardManager.Controls;
internal class ClipTypeDataTemplateSelector : DataTemplateSelector
{
  public override DataTemplate? SelectTemplate(object item, DependencyObject container)
  {
    var clipDto = (ClipDto) item;
    var pluginsService = ContainerLocator.Container.Resolve<IPluginsService>();
    var plugin = pluginsService[clipDto.PluginId];
    return plugin?.RepresentationDataDataTemplate;
  }
}
