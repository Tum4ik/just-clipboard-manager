using System.Collections.Immutable;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Prism.Events;
using Tum4ik.JustClipboardManager.Data;
using Tum4ik.JustClipboardManager.Data.Models;
using Tum4ik.JustClipboardManager.Data.Repositories;
using Tum4ik.JustClipboardManager.Events;
using Tum4ik.JustClipboardManager.PluginDevKit;
using Tum4ik.JustClipboardManager.PluginDevKit.Services;

namespace Tum4ik.JustClipboardManager.Services;
internal class ClipboardService : IClipboardService
{
  private readonly IClipRepository _clipRepository;
  private readonly IPluginsService _pluginsService;

  public ClipboardService(IEventAggregator eventAggregator,
                          IClipRepository clipRepository,
                          IPluginsService pluginsService)
  {
    _clipRepository = clipRepository;
    _pluginsService = pluginsService;

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

  private IReadOnlyCollection<IPlugin> _plugins;
  private ImmutableList<string> _pluginFormats;


  public void Paste(ICollection<FormattedDataObject> formattedDataObjects)
  {
    var dataObject = new DataObject(new OrderedDataStore());
    var restoredByPlugin = false;
    foreach (var formattedDataObject in formattedDataObjects)
    {
      object? data;

      if (!restoredByPlugin && _pluginFormats.Contains(formattedDataObject.Format))
      {
        var plugin = _plugins.First(p => p.Format == formattedDataObject.Format);
        data = plugin.RestoreData(formattedDataObject.Data, formattedDataObject.DataDotnetType);
        restoredByPlugin = true;
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
    _pluginFormats = _plugins.Select(p => p.Format).ToImmutableList();
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
             && p.Format == pluginFormat
      );
      if (plugin is null || string.IsNullOrEmpty(plugin.Id))
      {
        return;
      }

      var formattedDataObjects = new List<FormattedDataObject>();
      string? searchLabel = null;
      var representationData = Array.Empty<byte>();
      string? representationDataDotnetType = null;
      for (var i = 0; i < formats.Length; i++)
      {
        var format = formats[i];
        if (new[] {
            DataFormats.EnhancedMetafile, DataFormats.MetafilePicture, "FileContents"
          }.Contains(format))
        {
          // ignore unsupported formats
          continue;
        }

        object data;
        try
        {
          data = dataObject.GetData(format);
        }
        catch (COMException e)
        {
          Analytics.TrackEvent("Get Data Problem", new Dictionary<string, string>
          {
            { "DataFormat", format },
            { "Message", e.Message }
          });
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
          var clipData = plugin.ProcessData(data);
          if (clipData is null)
          {
            return;
          }
          dataBytes = clipData.Data;
          representationData = clipData.RepresentationData;
          representationDataDotnetType = dataType.ToString();
          searchLabel = clipData.SearchLabel;
        }
        else
        {
          dataBytes = GetDataBytes(data);
        }



        /*if (dataObject.GetDataPresent("Scalable Vector Graphics"))
        {

        }
        else*//*

        else if (dataObject.GetDataPresent(typeof(Bitmap)))
        {
          var representationDataObject = dataObject.GetData(typeof(Bitmap));
          representationData = GetBitmapBytes((Bitmap) representationDataObject);
        }
        else if (dataObject.GetDataPresent(DataFormats.FileDrop))
        {
          var filePaths = (string[]) dataObject.GetData(DataFormats.FileDrop);
          if (filePaths.Length == 1)
          {
            searchLabel = filePaths[0];
            representationData = GetStringBytes(filePaths[0]);
          }
          else
          {
            searchLabel = string.Join(";", filePaths);
            representationData = GetStringArrayBytes(filePaths);
          }
        }

    }*/


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

      if (representationData.Length <= 0 || representationDataDotnetType is null)
      {
        return;
      }

      var clip = new Clip
      {
        PluginId = plugin.Id,
        FormattedDataObjects = formattedDataObjects,
        RepresentationData = representationData,
        RepresentationDataDotnetType = representationDataDotnetType,
        SearchLabel = searchLabel
      };

      await _clipRepository.AddAsync(clip).ConfigureAwait(false);
    }
    catch (COMException e)
    {
      Crashes.TrackError(e);
    }
  }

  private static byte[] GetDataBytes(object data)
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

  private static object? GetDataFromBytes(FormattedDataObject formattedDataObject)
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


  private static byte[] UnrecognizedTypeBytes(object data)
  {
    Analytics.TrackEvent("Unable to save data", new Dictionary<string, string>
    {
      { "Data Type", data.GetType().ToString() }
    });
    return Array.Empty<byte>();
  }

  private static object? UnrecognizedDotnetType(string name)
  {
    Analytics.TrackEvent("Unable to load data", new Dictionary<string, string>
    {
      { ".NET Type", name }
    });
    return null;
  }
}
