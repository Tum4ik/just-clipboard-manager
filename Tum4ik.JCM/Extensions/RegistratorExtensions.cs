using DryIoc;

namespace Tum4ik.JustClipboardManager.Extensions;
internal static class RegistratorExtensions
{
  public static void RegisterViewWithViewModel<TView, TViewModel>(this IRegistrator registrator,
                                                                  IReuse? reuse = null)
    where TView : class
    where TViewModel : class
  {
    var resultReuse = reuse ?? Reuse.Transient;
    registrator.Register<TView>(resultReuse);
    registrator.Register<TViewModel>(resultReuse);
  }
}
