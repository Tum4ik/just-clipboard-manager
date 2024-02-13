using System.Windows;
using Tum4ik.StinimGen.Attributes;

namespace Tum4ik.JustClipboardManager.Ioc.Wrappers;

[IIFor(typeof(Clipboard), WrapperClassName = "ClipboardWrapper")]
internal partial interface IClipboard
{
}
