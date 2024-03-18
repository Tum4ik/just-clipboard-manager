using Tum4ik.JustClipboardManager.Data.Models;

namespace Tum4ik.JustClipboardManager.Services;
internal interface IPasteWindowService
{
  nint WindowHandle { get; }
  Task<PasteWindowResult?> ShowWindowAsync(nint targetWindowToPasteHandle);
  void HideWindow();
}


internal class PasteWindowResult
{
  public required ICollection<FormattedDataObject> FormattedDataObjects { get; set; }
  public string? AdditionalInfo { get; set; }
}
