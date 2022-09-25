using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace Tum4ik.JustClipboardManager.Data;

/// <summary>
/// Based on DataStore class from PresentationCore assembly.
/// </summary>
internal class OrderedDataStore : IDataObject
{
  private readonly OrderedDictionary _data = new();


  public object GetData(string format)
  {
    return GetData(format, true);
  }


  public object GetData(string format, bool autoConvert)
  {
    var aspect = DVASPECT.DVASPECT_CONTENT;
    var index = -1;

    var dataStoreEntry = FindDataStoreEntry(format, aspect, index);

    var baseVar = GetDataFromDataStoreEntry(dataStoreEntry, format);

    var original = baseVar;

    if (autoConvert
        && (dataStoreEntry == null || dataStoreEntry.AutoConvert)
        && (baseVar == null || baseVar is MemoryStream))
    {
      var mappedFormats = GetMappedFormats(format);
      if (mappedFormats != null)
      {
        for (int i = 0; i < mappedFormats.Length; i++)
        {
          if (!IsFormatEqual(format, mappedFormats[i]))
          {
            var foundDataStoreEntry = FindDataStoreEntry(mappedFormats[i], aspect, index);

            baseVar = GetDataFromDataStoreEntry(foundDataStoreEntry, mappedFormats[i]);

            if (baseVar != null && !(baseVar is MemoryStream))
            {
              if (baseVar is BitmapSource || SystemDrawingHelper_IsBitmap(baseVar))
              {
                // Ensure Bitmap(BitmapSource or System.Drawing.Bitmap) data which
                // match with the requested format.
                baseVar = EnsureBitmapDataFromFormat(format, autoConvert, baseVar);
              }

              original = null;
              break;
            }
          }
        }
      }
    }

    if (original is not null)
    {
      return original;
    }

    return baseVar;
  }


  public object GetData(Type format)
  {
    return GetData(format.FullName!);
  }


  public bool GetDataPresent(string format)
  {
    return GetDataPresent(format, true);
  }


  public bool GetDataPresent(string format, bool autoConvert)
  {
    var aspect = DVASPECT.DVASPECT_CONTENT;
    var index = -1;

    if (!autoConvert)
    {
      if (!_data.Contains(format))
      {
        return false;
      }

      var entries = (DataStoreEntry[]) _data[format];
      DataStoreEntry? dse = null;
      DataStoreEntry? naturalDse = null;

      // Find the entry with the given aspect and index
      for (var i = 0; i < entries.Length; i++)
      {
        var entry = entries[i];
        if (entry.Aspect == aspect && (index == -1 || entry.Index == index))
        {
          dse = entry;
          break;
        }
        if (entry.Aspect == DVASPECT.DVASPECT_CONTENT && entry.Index == 0)
        {
          naturalDse = entry;
        }
      }

      // If we couldn't find a specific entry, we'll use
      // aspect == Content and index == 0.
      if (dse is null && naturalDse is not null)
      {
        dse = naturalDse;
      }

      // If we still didn't find data, return false.
      return dse != null;
    }
    else
    {
      var formats = GetFormats(autoConvert);
      for (var i = 0; i < formats.Length; i++)
      {
        if (IsFormatEqual(format, formats[i]))
        {
          return true;
        }
      }
      return false;
    }
  }


  public bool GetDataPresent(Type format)
  {
    return GetDataPresent(format.FullName!);
  }


  public string[] GetFormats()
  {
    return GetFormats(true);
  }


  public string[] GetFormats(bool autoConvert)
  {
    //************************************************************
    // important! this is cached security information. It will
    //            remain valid for this function, but do not
    //            let this value leak outside of this function.
    //
    var serializationCheckFailedForThisFunction = false;
    var baseVar = new string[_data.Keys.Count];
    _data.Keys.CopyTo(baseVar, 0);

    if (autoConvert)
    {
      var formats = new ArrayList();

      for (int baseFormatIndex = 0; baseFormatIndex < baseVar.Length; baseFormatIndex++)
      {
        var entries = (DataStoreEntry[]) _data[baseVar[baseFormatIndex]];
        var canAutoConvert = true;

        for (int dataStoreIndex = 0; dataStoreIndex < entries.Length; dataStoreIndex++)
        {
          if (!entries[dataStoreIndex].AutoConvert)
          {
            canAutoConvert = false;
            break;
          }
        }

        if (canAutoConvert)
        {
          var cur = GetMappedFormats(baseVar[baseFormatIndex]);
          for (int mappedFormatIndex = 0; mappedFormatIndex < cur.Length; mappedFormatIndex++)
          {
            bool anySerializationFailure = false;
            for (int dataStoreIndex = 0;
                !anySerializationFailure
                  &&
                dataStoreIndex < entries.Length;
                dataStoreIndex++)
            {
              if (IsFormatAndDataSerializable(cur[mappedFormatIndex], entries[dataStoreIndex].Data)
                  && serializationCheckFailedForThisFunction)
              {
                serializationCheckFailedForThisFunction = true;
                anySerializationFailure = true;
              }
            }
            if (!anySerializationFailure)
            {
              formats.Add(cur[mappedFormatIndex]);
            }
          }
        }
        else
        {
          if (!serializationCheckFailedForThisFunction)
          {
            formats.Add(baseVar[baseFormatIndex]);
          }
        }
      }

      var temp = new string[formats.Count];
      formats.CopyTo(temp, 0);
      baseVar = GetDistinctStrings(temp);
    }

    return baseVar;
  }


  public void SetData(object data)
  {
    if (data is ISerializable && !_data.Contains(DataFormats.Serializable))
    {
      SetData(DataFormats.Serializable, data);
    }

    SetData(data.GetType(), data);
  }


  public void SetData(string format, object data)
  {
    SetData(format, data, true);
  }


  public void SetData(string format, object data, bool autoConvert)
  {
    // We do not have proper support for Dibs, so if the user explicitly asked
    // for Dib and provided a Bitmap object we can't convert.  Instead, publish as an HBITMAP
    // and let the system provide the conversion for us.
    //
    if (IsFormatEqual(format, DataFormats.Dib) 
        && autoConvert 
        && (SystemDrawingHelper_IsBitmap(data) 
        || data is BitmapSource))
    {
      format = DataFormats.Bitmap;
    }

    var aspect = DVASPECT.DVASPECT_CONTENT;
    var index = 0;

    var dse = new DataStoreEntry(data, autoConvert, aspect, index);
    var datalist = (DataStoreEntry[]) _data[format];

    if (datalist == null)
    {
      datalist = (DataStoreEntry[]) Array.CreateInstance(typeof(DataStoreEntry), 1);
    }
    else
    {
      var newlist = (DataStoreEntry[]) Array.CreateInstance(typeof(DataStoreEntry), datalist.Length + 1);
      datalist.CopyTo(newlist, 1);
      datalist = newlist;
    }

    datalist[0] = dse;
    _data[format] = datalist;
  }


  public void SetData(Type format, object data)
  {
    SetData(format.FullName!, data);
  }


  private DataStoreEntry? FindDataStoreEntry(string format, DVASPECT aspect, int index)
  {
    var dataStoreEntries = _data[format] as DataStoreEntry[];
    DataStoreEntry? dataStoreEntry = null;
    DataStoreEntry? naturalDataStoreEntry = null;

    // Find the entry with the given aspect and index
    if (dataStoreEntries is not null)
    {
      for (var i = 0; i < dataStoreEntries.Length; i++)
      {
        var entry = dataStoreEntries[i];
        if (entry.Aspect == aspect 
            && (index == -1 || entry.Index == index))
        {
          dataStoreEntry = entry;
          break;
        }
        if (entry.Aspect == DVASPECT.DVASPECT_CONTENT && entry.Index == 0)
        {
          naturalDataStoreEntry = entry;
        }
      }
    }

    // If we couldn't find a specific entry, we'll use
    // aspect == Content and index == 0.
    if (dataStoreEntry is null && naturalDataStoreEntry is not null)
    {
      dataStoreEntry = naturalDataStoreEntry;
    }

    return dataStoreEntry;
  }


  private object? GetDataFromDataStoreEntry(DataStoreEntry? dataStoreEntry, string format)
  {
    if (dataStoreEntry is not null)
    {
      return dataStoreEntry.Data;
    }

    return null;
  }


  /// <summary>
  /// Ensure returning Bitmap(BitmapSource or System.Drawing.Bitmap) data that base
  /// on the passed Bitmap format parameter.
  /// Bitmap data will be converted if the data mismatch with the format in case of
  /// autoConvert is "true", but return null if autoConvert is "false".
  /// </summary>
  private static object? EnsureBitmapDataFromFormat(string format, bool autoConvert, object data)
  {
    var bitmapData = data;

    if (data is BitmapSource && IsFormatEqual(format, SystemDrawingBitmapFormat))
    {
      // Data is BitmapSource, but have the mismatched System.Drawing.Bitmap format
      if (autoConvert)
      {
        // Convert data from BitmapSource to SystemDrawingBitmap
        bitmapData = SystemDrawingHelper_GetBitmap(data);
      }
      else
      {
        bitmapData = null;
      }
    }
    else if (SystemDrawingHelper_IsBitmap(data) &&
            (IsFormatEqual(format, DataFormats.Bitmap) || IsFormatEqual(format, SystemBitmapSourceFormat)))
    {
      // Data is System.Drawing.Bitmap, but have the mismatched BitmapSource format
      if (autoConvert)
      {
        // Create BitmapSource instance from System.Drawing.Bitmap
        var hbitmap = SystemDrawingHelper_GetHBitmapFromBitmap(data);
        bitmapData = Imaging.CreateBitmapSourceFromHBitmap(hbitmap, IntPtr.Zero,Int32Rect.Empty,null);
        DeleteObject(hbitmap);
      }
      else
      {
        bitmapData = null;
      }
    }

    return bitmapData;
  }


  /// <summary>
  /// Determines if the format/data combination is likely to
  /// be serialized. This is only likely, because this isn't a
  /// clone of the logic to determine the serialization code
  /// path. The idea is to determine if the format is likely
  /// to be serialized and do an early security check. If that
  /// check fails, then we will omit the format from the list
  /// of available clipboard formats.
  ///
  /// Note: at the time of serialization the correct security
  /// check will be performed, guaranteeing the security of
  /// the data, however that will cause the clipboard to report
  /// that there are no valid data items.
  /// </summary>
  /// <param name="format">Clipboard format to check</param>
  /// <param name="data">Clipboard data to check</param>
  /// <returns>
  /// true if the data is likely to be serializated
  /// through CLR serialization
  /// </returns>
  private static bool IsFormatAndDataSerializable(string format, object data)
  {
    return IsFormatEqual(format, DataFormats.Serializable)
      || data is ISerializable
      || (data != null && data.GetType().IsSerializable);
  }


  /// <summary>
  /// Return true if the format string are equal(Case-senstive).
  /// </summary>
  private static bool IsFormatEqual(string format1, string format2)
  {
    return string.CompareOrdinal(format1, format2) == 0;
  }


  private const string FileName = "FileName";
  private const string FileNameW = "FileNameW";
  private const string SystemDrawingBitmapFormat = "System.Drawing.Bitmap";
  private const string SystemBitmapSourceFormat = "System.Windows.Media.Imaging.BitmapSource";
  private const string SystemDrawingImagingMetafileFormat = "System.Drawing.Imaging.Metafile";

  /// <summary>
  /// Returns all the "synonyms" for the specified format.
  /// </summary>
  private static string[] GetMappedFormats(string format)
  {
    if (format == null)
    {
      return Array.Empty<string>();
    }

    if (IsFormatEqual(format, DataFormats.Text)
        || IsFormatEqual(format, DataFormats.UnicodeText)
        || IsFormatEqual(format, DataFormats.StringFormat))
    {
      return new string[]
      {
        DataFormats.Text,
        DataFormats.UnicodeText,
        DataFormats.StringFormat,
      };
    }

    if (IsFormatEqual(format, DataFormats.FileDrop)
        || IsFormatEqual(format, FileName)
        || IsFormatEqual(format, FileNameW))
    {
      return new string[]
      {
        DataFormats.FileDrop,
        FileNameW,
        FileName,
      };
    }

    // Get the System.Drawing.Bitmap string instead of getting it from typeof.
    // So we won't load System.Drawing.dll module here.
    if (IsFormatEqual(format, DataFormats.Bitmap)
        //|| IsFormat.Equals(format, DataFormats.Dib)
        || IsFormatEqual(format, SystemBitmapSourceFormat)
        || IsFormatEqual(format, SystemDrawingBitmapFormat))
    {
      return new string[]
      {
        DataFormats.Bitmap,
        SystemDrawingBitmapFormat,
        SystemBitmapSourceFormat
        //DataFormats.Dib,
      };
    }

    if (IsFormatEqual(format, DataFormats.EnhancedMetafile)
        || IsFormatEqual(format, SystemDrawingImagingMetafileFormat))
    {
      return new string[]
      {
        DataFormats.EnhancedMetafile,
        SystemDrawingImagingMetafileFormat
      };
    }

    return new string[] { format };
  }


  /// <summary>
  /// Retrieves a list of distinct strings from the array.
  /// </summary>
  private static string[] GetDistinctStrings(string[] formats)
  {
    var distinct = new ArrayList();
    for (int i = 0; i < formats.Length; i++)
    {
      var formatString = formats[i];
      if (!distinct.Contains(formatString))
      {
        distinct.Add(formatString);
      }
    }

    var distinctStrings = new string[distinct.Count];
    distinct.CopyTo(distinctStrings, 0);

    return distinctStrings;
  }


  private static readonly Type? s_systemDrawingHelperType = Type.GetType("MS.Internal.SystemDrawingHelper, PresentationCore");
  private static readonly MethodInfo? s_isBitmapMethod = s_systemDrawingHelperType?.GetMethod("IsBitmap", BindingFlags.NonPublic | BindingFlags.Static, new Type[] { typeof(object) });
  private static readonly MethodInfo? s_getBitmapMethod = s_systemDrawingHelperType?.GetMethod("GetBitmap", BindingFlags.NonPublic | BindingFlags.Static, new Type[] { typeof(object) });
  private static readonly MethodInfo? s_getHBitmapFromBitmapMethod = s_systemDrawingHelperType?.GetMethod("GetHBitmapFromBitmap", BindingFlags.NonPublic | BindingFlags.Static, new Type[] { typeof(object) });


  private static bool SystemDrawingHelper_IsBitmap(object data)
  {
    return (bool) (s_isBitmapMethod?.Invoke(null, new object[] { data }) ?? false);
  }


  private static object? SystemDrawingHelper_GetBitmap(object data)
  {
    return s_getBitmapMethod?.Invoke(null, new object[] { data });
  }


  private static IntPtr SystemDrawingHelper_GetHBitmapFromBitmap(object data)
  {
    return (IntPtr) (s_getHBitmapFromBitmapMethod?.Invoke(null, new object[] { data }) ?? IntPtr.Zero);
  }


  private sealed class DataStoreEntry
  {
    public DataStoreEntry(object data, bool autoConvert, DVASPECT aspect, int index)
    {
      Data = data;
      AutoConvert = autoConvert;
      Aspect = aspect;
      Index = index;
    }


    public object Data { get; set; }
    public bool AutoConvert { get; }
    public DVASPECT Aspect { get; }
    public int Index { get; }
  }


  /// <summary>
  /// Deletes a logical pen, brush, font, bitmap, region, or palette, freeing all system resources associated 
  /// with the object. After the object is deleted, the specified handle is no longer valid.
  /// </summary>
  /// <param name="hObject">A handle to a logical pen, brush, font, bitmap, region, or palette.</param>
  /// <returns>
  ///   <para>If the function succeeds, the return value is nonzero.</para>
  ///   <para>If the specified handle is not valid or is currently selected into a DC, the return value is zero.</para>
  /// </returns>
  /// <remarks>
  ///   <para>Do not delete a drawing object (pen or brush) while it is still selected into a DC.</para>
  ///   <para>When a pattern brush is deleted, the bitmap associated with the brush is not deleted. The bitmap must be deleted independently.</para>
  /// </remarks>
  [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
  [return: MarshalAs(UnmanagedType.Bool)]
  private static extern bool DeleteObject([In] IntPtr hObject);
}


[Flags]
public enum DVASPECT
{
  DVASPECT_CONTENT = 1,
  DVASPECT_THUMBNAIL = 2,
  DVASPECT_ICON = 4,
  DVASPECT_DOCPRINT = 8
}
