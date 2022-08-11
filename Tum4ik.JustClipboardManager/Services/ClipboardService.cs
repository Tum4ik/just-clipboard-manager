using System.IO;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
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


  private readonly SemaphoreSlim _semaphore = new(1, 1);
  private bool _clipboardChangedByThisService;


  private async Task OnClipboardChanged()
  {
    if (_clipboardChangedByThisService)
    {
      return;
    }

    var dataObject = _clipboard.GetDataObject();
    var formats = dataObject.GetFormats();
    var clipData = GetDataObjectBytes(dataObject);

    try
    {
      await _semaphore.WaitAsync().ConfigureAwait(false);

      var dbFormats = await _dbContext.ClipTypes
        .Where(ct => formats.Contains(ct.Name))
        .ToListAsync()
        .ConfigureAwait(false);

      var formatsToCreate = formats.Except(dbFormats.Select(dbf => dbf.Name));
      foreach (var newFormat in formatsToCreate)
      {
        var newClipType = new ClipType { Name = newFormat };
        await _dbContext.ClipTypes.AddAsync(newClipType).ConfigureAwait(false);
        dbFormats.Add(newClipType);
      }
      
      var newClip = new Clip
      {
        ClipTypes = dbFormats,
        Data = clipData
      };

      await _dbContext.Clips.AddAsync(newClip).ConfigureAwait(false);
      await _dbContext.SaveChangesAsync().ConfigureAwait(false);
    }
    finally
    {
      _semaphore.Release();
    }
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
