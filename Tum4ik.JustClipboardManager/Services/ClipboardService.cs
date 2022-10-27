using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Tum4ik.EventAggregator;
using Tum4ik.JustClipboardManager.Data;
using Tum4ik.JustClipboardManager.Data.Models;
using Tum4ik.JustClipboardManager.Data.Repositories;
using Tum4ik.JustClipboardManager.Events;

namespace Tum4ik.JustClipboardManager.Services;
internal class ClipboardService : IClipboardService
{
  private readonly IClipboard _clipboard;
  private readonly IClipRepository _clipRepository;

  public ClipboardService(IClipboard clipboard,
                          IEventAggregator eventAggregator,
                          IClipRepository clipRepository)
  {
    _clipboard = clipboard;
    _clipRepository = clipRepository;

    eventAggregator.GetEvent<ClipboardChangedEvent>().Subscribe(OnClipboardChanged, ThreadOption.MainThread);
  }


  /// <summary>
  /// When this service modifies the clipboard with data to be pasted, the notification about changing data in the 
  /// system clipboard will be sent and the <see cref="OnClipboardChanged"/> method will be called.
  /// To prevent that this marker is used. If the data object contains this marker, it means the data is pushed to
  /// the system clipboard by this service and that must not be processed.
  /// </summary>
  private const string ChangeMarker = "JCM_change_{AD3D5E08-A1FA-4602-AF24-94C4ADDBCA78}";


  public void Paste(ICollection<FormattedDataObject> formattedDataObjects)
  {
    var dataObject = new DataObject(new OrderedDataStore());
    foreach (var formattedDataObject in formattedDataObjects)
    {
      var data = GetDataFromBytes(formattedDataObject);
      dataObject.SetData(formattedDataObject.Format, data);
    }

    dataObject.SetData(ChangeMarker, new());
    _clipboard.SetDataObject(dataObject);
  }


  private async Task OnClipboardChanged()
  {
    try
    {
      await SaveClipAsync().ConfigureAwait(false);
    }
    catch (Exception e)
    {
      Crashes.TrackError(e);
    }
  }


  private async Task SaveClipAsync()
  {
    var dataObject = _clipboard.GetDataObject();
    var formats = dataObject.GetFormats(false);
    if (formats.Length == 0 || formats.Contains(ChangeMarker))
    {
      return;
    }

    var formattedDataObjects = new List<FormattedDataObject>();
    var clipType = ClipType.Unrecognized;
    string? searchLabel = null;
    var representationData = Array.Empty<byte>();
    var eventProps = new Dictionary<string, string>();
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
          { "Data Format / Message", $"{format} / {e.Message}" }
        });
        continue;
      }

      var dataType = data.GetType();
      if (dataType == typeof(Metafile))
      {
        // ignore unsupported types
        continue;
      }

      var dataDotnetType = dataType.ToString();
      eventProps[format] = dataDotnetType;

      if (clipType == ClipType.Unrecognized)
      {
        if (dataObject.GetDataPresent("Scalable Vector Graphics"))
        {

        }
        else if (dataObject.GetDataPresent(DataFormats.UnicodeText))
        {
          var text = (string) dataObject.GetData(DataFormats.UnicodeText);
          if (string.IsNullOrWhiteSpace(text))
          {
            return;
          }
          clipType = ClipType.Text;
          text = text.Trim();
          searchLabel = text;
          representationData = GetStringBytes(text);
        }
        else if (dataObject.GetDataPresent(typeof(Bitmap)))
        {
          clipType = ClipType.Image;
          var representationDataObject = dataObject.GetData(typeof(Bitmap));
          representationData = GetBitmapBytes((Bitmap) representationDataObject);
        }
        else if (dataObject.GetDataPresent(DataFormats.FileDrop))
        {
          var filePaths = (string[]) dataObject.GetData(DataFormats.FileDrop);
          if (filePaths.Length == 1)
          {
            clipType = ClipType.SingleFile;
            searchLabel = filePaths[0];
            representationData = GetStringBytes(filePaths[0]);
          }
          else
          {
            clipType = ClipType.FilesList;
            searchLabel = string.Join(";", filePaths);
            representationData = GetStringArrayBytes(filePaths);
          }
        }
      }

      var dataBytes = GetDataBytes(data);
      if (dataBytes.Length > 0)
      {
        var formattedDataObject = new FormattedDataObject
        {
          Data = dataBytes,
          DataDotnetType = dataDotnetType,
          Format = format,
          FormatOrder = i
        };
        formattedDataObjects.Add(formattedDataObject);
      }
    }

    if (clipType == ClipType.Unrecognized)
    {
      Analytics.TrackEvent("Unrecognized Clip Type", eventProps);
    }

    var clip = new Clip
    {
      ClipType = clipType,
      FormattedDataObjects = formattedDataObjects,
      RepresentationData = representationData,
      SearchLabel = searchLabel
    };

    await _clipRepository.AddAsync(clip).ConfigureAwait(false);
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
      "System.Windows.Interop.InteropBitmap"=> GetBitmapSourceFromBytes(data),
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

//+-------------------------------------------------------------+
//|             Clipboard data formats to C# types              |
//+---------------------+---------------------------------------+
//|     Data Format     |                C# type                |
//+---------------------+---------------------------------------+
//| Text                | System.String                         |
//| UnicodeText         | System.String                         |
//| Rtf                 | System.String                         |
//| Html                | System.String                         |
//| Xaml                |                                       |
//| XamlPackage         |                                       |
//| OemText             | System.String                         |
//| StringFormat        | System.String                         |
//| CommaSeparatedValue | System.IO.MemoryStream                |
//| SymbolicLink        | System.String                         |
//| Bitmap              | System.Windows.Interop.InteropBitmap  |
//| Tiff                | System.Windows.Interop.InteropBitmap  |
//| Riff                | System.IO.MemoryStream                |
//| MetafilePicture     | System.IO.MemoryStream                |
//| WaveAudio           | System.IO.MemoryStream                |
//| Serializable        |                                       |
//| Palette             | System.IO.MemoryStream                |
//| Locale              | System.IO.MemoryStream                |
//| FileDrop            | System.String[] (Array of file paths) |
//| EnhancedMetafile    | System.Drawing.Imaging.Metafile       |
//| Dif                 | System.String                         |
//| Dib                 | System.IO.MemoryStream                |
//| PenData             | System.IO.MemoryStream                |
//+---------------------+---------------------------------------+
