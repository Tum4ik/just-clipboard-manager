using System.IO;
using Tum4ik.StinimGen.Attributes;

namespace Tum4ik.JustClipboardManager.Ioc.Wrappers;

[IIFor(typeof(Path), WrapperClassName = "PathWrapper")]
internal partial interface IPath
{
}
