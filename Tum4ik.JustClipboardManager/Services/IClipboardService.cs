using Tum4ik.JustClipboardManager.Data.Models;

namespace Tum4ik.JustClipboardManager.Services;
internal interface IClipboardService
{
  void Paste(ICollection<FormattedDataObject> formattedDataObjects);
}
