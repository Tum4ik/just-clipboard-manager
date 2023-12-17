using System.ComponentModel;
using System.Globalization;
using Tum4ik.JustClipboardManager.PluginDevKit.Models;

namespace Tum4ik.JustClipboardManager.PluginDevKit.Services;

public interface ISettingsService
{
  CultureInfo Language { get; set; }
  string Theme { get; set; }

  KeyBindingDescriptor HotkeyShowPasteWindow { get; set; }

  int RemoveClipsPeriod { get; set; }
  PeriodType RemoveClipsPeriodType { get; set; }
}


public enum PeriodType
{
  [Description("day")]
  Day,
  [Description("month")]
  Month,
  [Description("year")]
  Year
}
