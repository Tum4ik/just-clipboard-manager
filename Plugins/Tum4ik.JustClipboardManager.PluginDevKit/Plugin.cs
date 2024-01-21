using System.Reflection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Markup;
using Tum4ik.JustClipboardManager.PluginDevKit.Attributes;
using Tum4ik.JustClipboardManager.PluginDevKit.Exceptions;
using Tum4ik.JustClipboardManager.PluginDevKit.Models;
using Windows.ApplicationModel.DataTransfer;

namespace Tum4ik.JustClipboardManager.PluginDevKit;

public abstract class Plugin : IPlugin
{
  private readonly Assembly _pluginAssembly;
  private readonly string _xamlTemplateFileName;

  protected Plugin(Assembly pluginAssembly, string xamlTemplateFileName)
  {
    _pluginAssembly = pluginAssembly;
    _xamlTemplateFileName = xamlTemplateFileName;
  }

  private string? _id;
  public string Id => _id ??= GetId(GetType());

  public abstract IReadOnlyCollection<string> Formats { get; }

  private DataTemplate? _dataTemplate;
  public DataTemplate RepresentationDataDataTemplate => _dataTemplate ??= GetDataTemplate();

  public abstract Task<ClipData> ProcessDataAsync(DataPackageView dataPackageView);
  public abstract object? RestoreData(byte[] bytes, string? additionalInfo);
  public abstract object? RestoreRepresentationData(byte[] bytes, string? additionalInfo);


  internal static string GetId(Type pluginType)
  {
    var attr = pluginType.GetCustomAttribute<PluginAttribute>(true);
    if (attr is null)
    {
      throw new PluginInitializationException("Can't read the plugin attribute.");
    }
    return attr.Id;
  }


  internal DataTemplate GetDataTemplate()
  {
    var xamlFileStream = _pluginAssembly.GetManifestResourceStream(
      _pluginAssembly
        .GetManifestResourceNames()
        .Single(n => n.EndsWith(_xamlTemplateFileName, StringComparison.OrdinalIgnoreCase))
    );
    if (xamlFileStream is null)
    {
      throw new PluginInitializationException("XAML template is not found.");
    }
    using var streamReader = new StreamReader(xamlFileStream);
    var xamlText = streamReader.ReadToEnd();
    return (DataTemplate) XamlReader.Load(xamlText);
  }
}
