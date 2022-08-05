using System.Windows;
using Tum4ik.Attributes;

namespace Tum4ik.JustClipboardManager.Services;

[IIForStaticClass(typeof(Clipboard), "ClipboardWrapper")]
internal partial interface IClipboard
{
}
