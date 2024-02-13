using Tum4ik.StinimGen.Attributes;

namespace Tum4ik.JustClipboardManager.Ioc.Wrappers;

[IIFor(typeof(Environment), WrapperClassName = "EnvironmentWrapper")]
internal partial interface IEnvironment
{
}
