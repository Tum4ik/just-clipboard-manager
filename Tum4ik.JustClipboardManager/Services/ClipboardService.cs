using System.Collections.Immutable;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Data.Sqlite;
using Prism.Events;
using Tum4ik.JustClipboardManager.Data;
using Tum4ik.JustClipboardManager.Data.Models;
using Tum4ik.JustClipboardManager.Data.Repositories;
using Tum4ik.JustClipboardManager.Events;
using Tum4ik.JustClipboardManager.Services.Plugins;

namespace Tum4ik.JustClipboardManager.Services;
internal class ClipboardService : IClipboardService
{
  private readonly IClipRepository _clipRepository;
  private readonly IPluginsService _pluginsService;
  private readonly IHub _sentryHub;

  public ClipboardService(IEventAggregator eventAggregator,
                          IClipRepository clipRepository,
                          IPluginsService pluginsService,
                          IHub sentryHub)
  {
    _clipRepository = clipRepository;
    _pluginsService = pluginsService;
    _sentryHub = sentryHub;

    eventAggregator.GetEvent<ClipboardChangedEvent>().Subscribe(OnClipboardChanged, ThreadOption.UIThread);
  }


  /// <summary>
  /// When this service modifies the clipboard with data to be pasted, the notification about changing data in the 
  /// system clipboard will be sent and the <see cref="OnClipboardChanged"/> method will be called.
  /// To prevent that this marker is used. If the data object contains this marker, it means the data is pushed to
  /// the system clipboard by this service and that must not be processed.
  /// </summary>
  private const string ChangeMarker = "JCM_change_{AD3D5E08-A1FA-4602-AF24-94C4ADDBCA78}";


  public void Paste(ICollection<FormattedDataObject> formattedDataObjects, string? additionalInfo)
  {
    var dataObject = new DataObject(new OrderedDataStore());
    var restoredByPlugin = false;
    foreach (var formattedDataObject in formattedDataObjects)
    {
      object? data;

      if (!restoredByPlugin && _pluginsService.EnabledPluginFormats.Contains(formattedDataObject.Format))
      {
        var (pluginId, plugin) = _pluginsService
          .EnabledPlugins
          .First(p => p.Value.Formats.Contains(formattedDataObject.Format));
        try
        {
          data = plugin.RestoreData(formattedDataObject.Data, additionalInfo);
          restoredByPlugin = true;
        }
        catch (Exception e)
        {
          _sentryHub.CaptureException(e, scope =>
          {
            scope.AddBreadcrumb(
              message: "Exception when restore data for plugin on paste operation",
              category: "info",
              type: "info",
              dataPair: ("Plugin Id", pluginId.ToString())
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


  private async Task SaveClipAsync()
  {
    try
    {
      var dataObject = Clipboard.GetDataObject();
      var formats = dataObject.GetFormats(false);
      var pluginFormat = formats.Intersect(_pluginsService.EnabledPluginFormats).FirstOrDefault();
      if (formats.Length == 0 || formats.Contains(ChangeMarker) || pluginFormat is null)
      {
        return;
      }

      var (pluginId, plugin) = _pluginsService
        .EnabledPlugins
        .FirstOrDefault(p => p.Value.Formats.Contains(pluginFormat));
      if (plugin is null)
      {
        return;
      }

      _sentryHub.AddBreadcrumb(
        message: "Track plugin id",
        category: "info",
        type: "info",
        data: new Dictionary<string, string>
        {
          { "Plugin id", pluginId.ToString() }
        }
      );
      
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
          _sentryHub.CaptureException(e, scope => scope.AddBreadcrumb(
            message: "Get Data Problem",
            category: "info",
            type: "info",
            dataPair: ("DataFormat", format)
          ));
          continue;
        }

        if (data is null)
        {
          _sentryHub.CaptureMessage(
            $"Data is not available in the specified format: {format}.",
            SentryLevel.Warning
          );
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
        PluginId = pluginId,
        FormattedDataObjects = formattedDataObjects,
        RepresentationData = representationData,
        AdditionalInfo = additionalInfo,
        SearchLabel = searchLabel
      };

      await _clipRepository.AddAsync(clip).ConfigureAwait(false);
    }
    catch (COMException e)
    {
      _sentryHub.CaptureException(e, scope => scope.AddBreadcrumb(
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
    catch (SqliteException e)
    {
      var dbFilePath = AppDbContext.DbFilePath;
      _sentryHub.CaptureException(e, scope => scope.AddBreadcrumb(
        message: "SQLite exception when saving clip",
        category: "info",
        type: "info",
        data: new Dictionary<string, string>
        {
          { "DB file exists", File.Exists(dbFilePath).ToString() }
        }
      ));
    }
    catch (Exception e)
    {
      _sentryHub.CaptureException(e, scope => scope.AddBreadcrumb(
        message: "Unpredictable exception when saving clip",
        type: "info"
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
    _sentryHub.CaptureMessage("Unable to save data", scope => scope.AddBreadcrumb(
      message: "",
      category: "info",
      type: "info",
      dataPair: ("Data Type", data.GetType().ToString())
    ));
    return Array.Empty<byte>();
  }

  private object? UnrecognizedDotnetType(string name)
  {
    _sentryHub.CaptureMessage("Unable to load data", scope => scope.AddBreadcrumb(
      message: "",
      category: "info",
      type: "info",
      dataPair: (".NET Type", name)
    ));
    return null;
  }
}
