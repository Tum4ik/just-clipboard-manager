using Tum4ik.EventAggregator.Event;

namespace Tum4ik.JustClipboardManager.Events;

internal record PasteWindowResultEvent(string Payload) : IEvent<string>;
internal record ClipboardChangedEvent() : IEvent;
