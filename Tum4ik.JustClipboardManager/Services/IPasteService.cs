using Tum4ik.JustClipboardManager.Data.Models;

namespace Tum4ik.JustClipboardManager.Services;
internal interface IPasteService
{
  void PasteData(IntPtr targetWindowPtr, ICollection<FormattedDataObject> data);
}
