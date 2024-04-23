using System.Collections.Immutable;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using Prism.Events;
using Tum4ik.JustClipboardManager.Data;
using Tum4ik.JustClipboardManager.Data.Models;
using Tum4ik.JustClipboardManager.Data.Repositories;
using Tum4ik.JustClipboardManager.Events;
using Tum4ik.JustClipboardManager.PluginDevKit;

namespace Tum4ik.JustClipboardManager.Services;
internal class ClipboardService : IClipboardService
{
  private readonly IClipRepository _clipRepository;
  private readonly IPluginsService _pluginsService;
  private readonly Lazy<IHub> _sentryHub;

  public ClipboardService(IEventAggregator eventAggregator,
                          IClipRepository clipRepository,
                          IPluginsService pluginsService,
                          Lazy<IHub> sentryHub)
  {
    _clipRepository = clipRepository;
    _pluginsService = pluginsService;
    _sentryHub = sentryHub;

    OnPluginsChainUpdated();

    eventAggregator.GetEvent<ClipboardChangedEvent>().Subscribe(OnClipboardChanged, ThreadOption.UIThread);
    eventAggregator.GetEvent<PluginsChainUpdatedEvent>().Subscribe(OnPluginsChainUpdated);
  }


  /// <summary>
  /// When this service modifies the clipboard with data to be pasted, the notification about changing data in the 
  /// system clipboard will be sent and the <see cref="OnClipboardChanged"/> method will be called.
  /// To prevent that this marker is used. If the data object contains this marker, it means the data is pushed to
  /// the system clipboard by this service and that must not be processed.
  /// </summary>
  private const string ChangeMarker = "JCM_change_{AD3D5E08-A1FA-4602-AF24-94C4ADDBCA78}";

  private IReadOnlyCollection<IPlugin> _plugins = Array.Empty<IPlugin>();
  private ImmutableList<string> _pluginFormats = ImmutableList.Create<string>();


  public void Paste(ICollection<FormattedDataObject> formattedDataObjects, string? additionalInfo)
  {
    var dataObject = new DataObject(new OrderedDataStore());
    var restoredByPlugin = false;
    foreach (var formattedDataObject in formattedDataObjects)
    {
      object? data;

      if (!restoredByPlugin && _pluginFormats.Contains(formattedDataObject.Format))
      {
        var plugin = _plugins.First(p => p.Formats.Contains(formattedDataObject.Format));
        try
        {
          data = plugin.RestoreData(formattedDataObject.Data, additionalInfo);
          restoredByPlugin = true;
        }
        catch (Exception e)
        {
          _sentryHub.Value.CaptureException(e, scope =>
          {
            scope.AddBreadcrumb(
              message: "Exception when restore data for plugin on paste operation",
              category: "info",
              type: "info",
              dataPair: ("Plugin Id", plugin.Id!)
            );
          });
          data = GetDataFromBytes(formattedDataObject);
        }
      }
      else
      {
        data = GetDataFromBytes(formattedDataObject);
      }

      if (data is not null)
      {
        dataObject.SetData(formattedDataObject.Format, data);
      }
    }

    dataObject.SetData(ChangeMarker, new());
    Clipboard.SetDataObject(dataObject);
  }


  private void OnClipboardChanged()
  {
    SaveClipAsync().Await();
  }


  private void OnPluginsChainUpdated()
  {
    _plugins = _pluginsService.InstalledPlugins;
    _pluginFormats = _plugins.SelectMany(p => p.Formats).ToImmutableList();
  }


  private async Task SaveClipAsync()
  {
    try
    {
      var dataObject = Clipboard.GetDataObject();
      var formats = dataObject.GetFormats(false);
      var pluginFormat = formats.Intersect(_pluginFormats).FirstOrDefault();
      if (formats.Length == 0 || formats.Contains(ChangeMarker) || pluginFormat is null)
      {
        return;
      }

      var plugin = _plugins.FirstOrDefault(
        p => p.Id is not null
             && _pluginsService.IsPluginEnabled(p.Id)
             && p.Formats.Contains(pluginFormat)
      );
      if (plugin is null || string.IsNullOrEmpty(plugin.Id))
      {
        return;
      }

      var formattedDataObjects = new List<FormattedDataObject>();
      string? searchLabel = null;
      var representationData = Array.Empty<byte>();
      string? additionalInfo = null;
      for (var i = 0; i < formats.Length; i++)
      {
        var format = formats[i];
        if (new[] {
            DataFormats.EnhancedMetafile, DataFormats.MetafilePicture, "FileContents", "AsyncFlag"
          }.Contains(format))
        {
          // ignore unsupported or problematic formats
          continue;
        }

        object? data;
        try
        {
          data = dataObject.GetData(format);
        }
        catch (COMException e)
        {
          _sentryHub.Value.CaptureException(e, scope => scope.AddBreadcrumb(
            message: "Get Data Problem",
            category: "info",
            type: "info",
            dataPair: ("DataFormat", format)
          ));
          continue;
        }

        if (data is null)
        {
          continue;
        }

        var dataType = data.GetType();
        if (dataType == typeof(Metafile))
        {
          // ignore unsupported types
          continue;
        }

        byte[] dataBytes;
        if (format == pluginFormat)
        {
          var clipData = plugin.ProcessData(dataObject);
          if (clipData is null)
          {
            return;
          }
          dataBytes = clipData.Data;
          representationData = clipData.RepresentationData;
          additionalInfo = clipData.AdditionalInfo;
          searchLabel = clipData.SearchLabel;
        }
        else
        {
          dataBytes = GetDataBytes(data);
        }

        if (dataBytes.Length > 0)
        {
          var formattedDataObject = new FormattedDataObject
          {
            Data = dataBytes,
            DataDotnetType = dataType.ToString(),
            Format = format,
            FormatOrder = i
          };
          formattedDataObjects.Add(formattedDataObject);
        }
      }

      if (representationData.Length <= 0)
      {
        return;
      }

      var clip = new Clip
      {
        PluginId = plugin.Id,
        FormattedDataObjects = formattedDataObjects,
        RepresentationData = representationData,
        AdditionalInfo = additionalInfo,
        SearchLabel = searchLabel
      };

      await _clipRepository.AddAsync(clip).ConfigureAwait(false);
    }
    catch (COMException e)
    {
      _sentryHub.Value.CaptureException(e, scope => scope.AddBreadcrumb(
        message: "COM exception when saving clip",
        category: "info",
        type: "info",
        data: new Dictionary<string, string>
        {
          { "ErrorCode", e.ErrorCode.ToString(CultureInfo.InvariantCulture) },
          { "HResult", e.HResult.ToString(CultureInfo.InvariantCulture) }
        }
      ));
    }
    catch (Exception e)
    {
      _sentryHub.Value.CaptureException(e, scope => scope.AddBreadcrumb(
        message: "Unpredictable exception when saving clip",
        category: "info",
        type: "info",
        data: null
      ));
    }
  }

  private byte[] GetDataBytes(object data)
  {
    return data switch
    {
      string d => GetStringBytes(d),
      string[] d => GetStringArrayBytes(d),
      BitmapSource d => GetBitmapSourceBytes(d),
      Bitmap d => GetBitmapBytes(d),
      MemoryStream d => GetMemoryStreamBytes(d),
      bool d => GetBooleanBytes(d),
      _ => UnrecognizedTypeBytes(data)
    };
  }

  private object? GetDataFromBytes(FormattedDataObject formattedDataObject)
  {
    var data = formattedDataObject.Data;
    return formattedDataObject.DataDotnetType switch
    {
      "System.String" => GetStringFromBytes(data),
      "System.String[]" => GetStringArrayFromBytes(data),
      "System.Windows.Interop.InteropBitmap" => GetBitmapSourceFromBytes(data),
      "System.Drawing.Bitmap" => GetBitmapFromBytes(data),
      "System.IO.MemoryStream" => GetMemoryStreamFromBytes(data),
      "System.Boolean" => GetBooleanFromBytes(data),
      _ => UnrecognizedDotnetType(formattedDataObject.DataDotnetType)
    };
  }


  private static byte[] GetStringBytes(string data)
  {
    return Encoding.UTF8.GetBytes(data);
  }

  private static string GetStringFromBytes(byte[] bytes)
  {
    return Encoding.UTF8.GetString(bytes);
  }


  private static byte[] GetStringArrayBytes(string[] data)
  {
    var str = string.Join(";", data);
    return Encoding.UTF8.GetBytes(str);
  }

  private static string[] GetStringArrayFromBytes(byte[] bytes)
  {
    var str = Encoding.UTF8.GetString(bytes);
    return str.Split(";");
  }


  private static byte[] GetBitmapSourceBytes(BitmapSource data)
  {
    using var memoryStream = new MemoryStream();
    var bitmapEncoder = new PngBitmapEncoder();
    bitmapEncoder.Frames.Add(BitmapFrame.Create(data));
    bitmapEncoder.Save(memoryStream);
    return memoryStream.ToArray();
  }

  private static BitmapSource GetBitmapSourceFromBytes(byte[] bytes)
  {
    using var memoryStream = new MemoryStream(bytes);
    return BitmapFrame.Create(memoryStream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
  }


  private static byte[] GetBitmapBytes(Bitmap data)
  {
    using var memoryStream = new MemoryStream();
    data.Save(memoryStream, ImageFormat.Png);
    return memoryStream.ToArray();
  }

  private static Bitmap GetBitmapFromBytes(byte[] bytes)
  {
    using var memoryStream = new MemoryStream(bytes);
    return new(memoryStream);
  }


  private static byte[] GetMemoryStreamBytes(MemoryStream data)
  {
    var bytes = data.ToArray();
    data.Dispose();
    return bytes;
  }

  private static MemoryStream GetMemoryStreamFromBytes(byte[] bytes)
  {
    return new(bytes);
  }


  private static byte[] GetBooleanBytes(bool data)
  {
    return BitConverter.GetBytes(data);
  }

  private static bool GetBooleanFromBytes(byte[] bytes)
  {
    return BitConverter.ToBoolean(bytes);
  }


  private byte[] UnrecognizedTypeBytes(object data)
  {
    _sentryHub.Value.CaptureMessage("Unable to save data", scope => scope.AddBreadcrumb(
      message: "",
      category: "info",
      type: "info",
      dataPair: ("Data Type", data.GetType().ToString())
    ));
    return Array.Empty<byte>();
  }

  private object? UnrecognizedDotnetType(string name)
  {
    _sentryHub.Value.CaptureMessage("Unable to load data", scope => scope.AddBreadcrumb(
      message: "",
      category: "info",
      type: "info",
      dataPair: (".NET Type", name)
    ));
    return null;
  }
}
