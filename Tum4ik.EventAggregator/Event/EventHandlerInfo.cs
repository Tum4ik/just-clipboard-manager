using System.Reflection;

namespace Tum4ik.EventAggregator.Event;
internal class EventHandlerInfo
{
  public EventHandlerInfo(Delegate handlerAction, ThreadOption threadOption, bool keepHandlerAlive)
  {
    ThreadOption = threadOption;
    if (keepHandlerAlive)
    {
      _handlerAction = handlerAction;
    }
    else
    {
      _weakReference = new WeakReference(handlerAction.Target);
      _methodInfo = handlerAction.GetMethodInfo();
      _delegateType = handlerAction.GetType();
    }
  }


  private readonly Delegate? _handlerAction;
  private readonly WeakReference? _weakReference;
  private readonly MethodInfo? _methodInfo;
  private readonly Type? _delegateType;


  public Delegate? HandlerAction => _handlerAction ?? GetHandlerAction();
  public ThreadOption ThreadOption { get; }
  public bool IsAlive => _handlerAction is not null || (_weakReference is not null && _weakReference.IsAlive);


  private Delegate? GetHandlerAction()
  {
    if (_weakReference is null || _methodInfo is null || _delegateType is null)
    {
      return null;
    }

    if (_methodInfo.IsStatic)
    {
      return _methodInfo.CreateDelegate(_delegateType);
    }

    var weakReferenceTarget = _weakReference.Target;
    if (weakReferenceTarget is not null)
    {
      return _methodInfo.CreateDelegate(_delegateType, weakReferenceTarget);
    }

    return null;
  }
}
