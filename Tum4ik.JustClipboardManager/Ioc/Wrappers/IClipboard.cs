using System.Windows;
using YT.IIGen.Attributes;

namespace Tum4ik.JustClipboardManager.Ioc.Wrappers;

[IIFor(typeof(Clipboard), "ClipboardWrapper")]
internal partial interface IClipboard
{
}
