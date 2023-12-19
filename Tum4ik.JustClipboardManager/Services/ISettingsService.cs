using System.ComponentModel;
using Tum4ik.JustClipboardManager.Data.Models;
using Tum4ik.JustClipboardManager.PluginDevKit.Services;

namespace Tum4ik.JustClipboardManager.Services;

internal interface ISettingsService : IPluginSettingsService
{
  KeyBindingDescriptor HotkeyShowPasteWindow { get; set; }

  int RemoveClipsPeriod { get; set; }
  PeriodType RemoveClipsPeriodType { get; set; }

  PasteWindowSnappingType PasteWindowSnappingType { get; set; }
  PasteWindowSnappingDisplayCorner PasteWindowSnappingDisplayCorner { get; set; }
}


public enum PeriodType
{
  [Description("day")] Day,
  [Description("month")] Month,
  [Description("year")] Year
}

internal enum PasteWindowSnappingType
{
  [Description("Mouse")] Mouse,
  [Description("Caret")] Caret,
  [Description("DisplayCorner")] DisplayCorner
}

internal enum PasteWindowSnappingDisplayCorner
{
  [Description("TopLeft")] TopLeft,
  [Description("TopRight")] TopRight,
  [Description("BottomLeft")] BottomLeft,
  [Description("BottomRight")] BottomRight
}
