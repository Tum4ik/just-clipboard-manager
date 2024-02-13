using System.Diagnostics;
using Tum4ik.StinimGen.Attributes;

namespace Tum4ik.JustClipboardManager.Ioc.Wrappers;

[IIFor(typeof(Process), WrapperClassName = "ProcessWrapper")]
internal partial interface IProcess
{
}
