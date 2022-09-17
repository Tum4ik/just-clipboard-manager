using Tum4ik.EventAggregator.Event;
using Tum4ik.JustClipboardManager.Data.Models;

namespace Tum4ik.JustClipboardManager.Events;
internal class PasteWindowResultEvent : PubSubEvent<ICollection<FormattedDataObject>>
{
}
