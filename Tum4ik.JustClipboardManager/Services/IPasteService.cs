using Tum4ik.JustClipboardManager.Data.Models;

namespace Tum4ik.JustClipboardManager.Services;
internal interface IPasteService
{
  void PasteData(nint targetWindowPtr, ICollection<FormattedDataObject> data);
}
