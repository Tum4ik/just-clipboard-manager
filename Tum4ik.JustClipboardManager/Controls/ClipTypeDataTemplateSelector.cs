using System.Windows;
using System.Windows.Controls;
using Microsoft.AppCenter.Analytics;
using Tum4ik.JustClipboardManager.Constants;
using Tum4ik.JustClipboardManager.Data.Models;

namespace Tum4ik.JustClipboardManager.Controls;
internal class ClipTypeDataTemplateSelector : DataTemplateSelector
{
  public override DataTemplate? SelectTemplate(object item, DependencyObject container)
  {
    var clip = (Clip) item;
    var templateName = clip.ClipType switch
    {
      ClipType.Unrecognized => DataTemplateNames.ClipTypeUndefined,
      ClipType.Text => DataTemplateNames.ClipTypeText,
      ClipType.Image => DataTemplateNames.ClipTypeImage,
      ClipType.Svg => DataTemplateNames.ClipTypeSvg,
      ClipType.SingleFile => DataTemplateNames.ClipTypeSingleFile,
      ClipType.FilesList => DataTemplateNames.ClipTypeFilesList,
      _ => UndefinedTemplate(clip.ClipType)
    };

    var resource = (container as FrameworkElement)?.FindResource(templateName);
    return resource as DataTemplate;
  }


  private static string UndefinedTemplate(ClipType clipType)
  {
    Analytics.TrackEvent("Undefined Data Template", new Dictionary<string, string>
    {
      { "Clip Type", clipType.ToString() }
    });
    return DataTemplateNames.ClipTypeUndefined;
  }
}
