using System.IO;
using System.Linq;
using System.Text.Json;
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
                          IEventSubscriber eventSubscriber,
                          AppDbContext dbContext)
  {
    _clipboard = clipboard;
    _dbContext = dbContext;

    eventSubscriber.Subscribe<ClipboardChangedEvent>(OnClipboardChanged, ThreadOption.MainThread);
  }


  private bool _clipboardChangedByThisService;


  private void OnClipboardChanged(ClipboardChangedEvent clipboardChanged)
  {
    if (_clipboardChangedByThisService)
    {
      return;
    }

    var dataObject = _clipboard.GetDataObject();
    var formats = dataObject.GetFormats();

    var dbFormats = _dbContext.ClipTypes
      .Where(ct => formats.Contains(ct.Name))
      .ToList();

    var formatsToCreate = formats.Except(dbFormats.Select(dbf => dbf.Name));
    foreach (var newFormat in formatsToCreate)
    {
      var newClipType = new ClipType { Name = newFormat };
      _dbContext.ClipTypes.Add(newClipType);
      dbFormats.Add(newClipType);
    }

    var clipData = GetDataObjectBytes(dataObject);
    var newClip = new Clip
    {
      ClipTypes = dbFormats,
      Data = clipData
    };

    _dbContext.Clips.Add(newClip);
    _dbContext.SaveChanges();
  }


  private static byte[] GetDataObjectBytes(object dataObject)
  {
    using var memoryStream = new MemoryStream();
    JsonSerializer.Serialize(memoryStream, dataObject);
    return memoryStream.ToArray();
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
//| Dib                 | System.Windows.Interop.InteropBitmap  |
//| PenData             | System.IO.MemoryStream                |
//+---------------------+---------------------------------------+
