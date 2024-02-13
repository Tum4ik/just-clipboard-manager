using System.IO;
using Tum4ik.StinimGen.Attributes;

namespace Tum4ik.JustClipboardManager.Ioc.Wrappers;

[IIFor(typeof(File), WrapperClassName = "FileWrapper")]
internal partial interface IFile
{
}
