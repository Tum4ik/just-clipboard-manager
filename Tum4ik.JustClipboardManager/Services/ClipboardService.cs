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
using Tum4ik.JustClipboardManager.Events;

namespace Tum4ik.JustClipboardManager.Services;
internal class ClipboardService : IClipboardService
{
  private readonly IClipboard _clipboard;
  private readonly AppDbContext _dbContext;

  public ClipboardService(IClipboard clipboard,
                          IEventAggregator eventAggregator,
                          AppDbContext dbContext)
  {
    _clipboard = clipboard;
    _dbContext = dbContext;

    eventAggregator.GetEvent<ClipboardChangedEvent>().Subscribe(OnClipboardChanged, ThreadOption.MainThread);
  }


  private bool _clipboardChangedByThisService;


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
    if (_clipboardChangedByThisService)
    {
      return;
    }

    var dataObject = _clipboard.GetDataObject();
    var formats = dataObject.GetFormats(false);
    if (formats.Length == 0)
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

    await _dbContext.Clips.AddAsync(clip).ConfigureAwait(false);
    await _dbContext.SaveChangesAsync().ConfigureAwait(false);
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
      _ => UnrecognizedTypeBytes(data)
    };
  }


  private static byte[] GetStringBytes(string data)
  {
    return Encoding.UTF8.GetBytes(data);
  }


  private static byte[] GetStringArrayBytes(string[] data)
  {
    var str = string.Join(";", data);
    return Encoding.UTF8.GetBytes(str);
  }


  private static byte[] GetBitmapSourceBytes(BitmapSource data)
  {
    using var memoryStream = new MemoryStream();
    var bitmapEncoder = new PngBitmapEncoder();
    bitmapEncoder.Frames.Add(BitmapFrame.Create(data));
    bitmapEncoder.Save(memoryStream);
    return memoryStream.ToArray();
  }


  private static byte[] GetBitmapBytes(Bitmap data)
  {
    using var memoryStream = new MemoryStream();
    data.Save(memoryStream, ImageFormat.Png);
    return memoryStream.ToArray();
  }


  private static byte[] GetMemoryStreamBytes(MemoryStream data)
  {
    var bytes = data.ToArray();
    data.Dispose();
    return bytes;
  }


  private static byte[] UnrecognizedTypeBytes(object data)
  {
    Analytics.TrackEvent("Unable to save data", new Dictionary<string, string>
    {
      { "Data Type", data.GetType().ToString() }
    });
    return Array.Empty<byte>();
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
