using Prism.Events;
using Tum4ik.JustClipboardManager.Data.Models;

namespace Tum4ik.JustClipboardManager.Events;
internal class PasteWindowResultEvent : PubSubEvent<ICollection<FormattedDataObject>>
{
}
