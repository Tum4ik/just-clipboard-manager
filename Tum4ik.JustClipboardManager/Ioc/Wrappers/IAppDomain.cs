using System.Reflection;
using Tum4ik.StinimGen.Attributes;

namespace Tum4ik.JustClipboardManager.Ioc.Wrappers;

[IIFor(typeof(AppDomain), WrapperClassName = "AppDomainWrapper", IsPartial = true)]
internal partial interface IAppDomain
{
  Assembly[] GetLoadedAssemblies();
}


partial class AppDomainWrapper
{
  public Assembly[] GetLoadedAssemblies()
  {
    return AppDomain.CurrentDomain.GetAssemblies().Where(p => !p.IsDynamic).ToArray();
  }
}
