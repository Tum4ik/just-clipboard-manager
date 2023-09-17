using Prism.Events;
using Tum4ik.JustClipboardManager.Data.Models;

namespace Tum4ik.JustClipboardManager.Events;
internal class PasteWindowResultEvent : PubSubEvent<PasteWindowResultPayload?>
{
}

internal class PasteWindowResultPayload
{
  public required ICollection<FormattedDataObject> FormattedDataObjects { get; set; }
  public string? AdditionalInfo { get; set; }
}
