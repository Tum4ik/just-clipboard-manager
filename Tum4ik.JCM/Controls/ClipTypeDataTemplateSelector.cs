using DryIoc;
using Tum4ik.JustClipboardManager.Data.Dto;
using Tum4ik.JustClipboardManager.PluginDevKit;

namespace Tum4ik.JustClipboardManager.Controls;
internal sealed class ClipTypeDataTemplateSelector : DataTemplateSelector
{
  private readonly IResolver _resolver;

  public ClipTypeDataTemplateSelector(IResolver resolver)
  {
    _resolver = resolver;
  }


  protected override DataTemplate SelectTemplateCore(object item)
  {
    var clipDto = (ClipDto) item;
    var plugin = _resolver.Resolve<IPlugin>(clipDto.PluginId);
    return plugin.RepresentationDataDataTemplate;
  }
}
