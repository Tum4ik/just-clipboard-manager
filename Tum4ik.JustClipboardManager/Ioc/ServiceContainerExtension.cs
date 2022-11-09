using Microsoft.Extensions.DependencyInjection;
using Prism.Ioc;

namespace Tum4ik.JustClipboardManager.Ioc;
internal class ServiceContainerExtension : IContainerExtension
{
  private readonly IServiceProvider _serviceProvider;

  public ServiceContainerExtension(IServiceProvider serviceProvider)
  {
    _serviceProvider = serviceProvider;
  }


  public void FinalizeExtension()
  {
    // nothing to do
  }


  private readonly NotSupportedException _useServiceCollection = new("Use Microsoft.Extensions.DependencyInjection.ServiceCollection");
  

  public object Resolve(Type type)
  {
    return _serviceProvider.GetRequiredService(type);
  }

  public object Resolve(Type type, params (Type Type, object Instance)[] parameters)
  {
    throw new NotSupportedException("Resolve with parameters is not supported.");
  }

  public object Resolve(Type type, string name)
  {
    throw new NotSupportedException("Resolve with name is not supported.");
  }

  public object Resolve(Type type, string name, params (Type Type, object Instance)[] parameters)
  {
    throw new NotSupportedException("Resolve with name is not supported.");
  }


  public IScopedProvider CreateScope() => throw new NotSupportedException();
  public IScopedProvider CurrentScope => throw new NotSupportedException();
  public IContainerRegistry RegisterInstance(Type type, object instance) => throw _useServiceCollection;
  public IContainerRegistry RegisterInstance(Type type, object instance, string name) => throw _useServiceCollection;
  public IContainerRegistry RegisterSingleton(Type from, Type to) => throw _useServiceCollection;
  public IContainerRegistry RegisterSingleton(Type from, Type to, string name) => throw _useServiceCollection;
  public IContainerRegistry RegisterSingleton(Type type, Func<object> factoryMethod) => throw _useServiceCollection;
  public IContainerRegistry RegisterSingleton(Type type, Func<IContainerProvider, object> factoryMethod) => throw _useServiceCollection;
  public IContainerRegistry RegisterManySingleton(Type type, params Type[] serviceTypes) => throw _useServiceCollection;
  public IContainerRegistry Register(Type from, Type to) => throw _useServiceCollection;
  public IContainerRegistry Register(Type from, Type to, string name) => throw _useServiceCollection;
  public IContainerRegistry Register(Type type, Func<object> factoryMethod) => throw _useServiceCollection;
  public IContainerRegistry Register(Type type, Func<IContainerProvider, object> factoryMethod) => throw _useServiceCollection;
  public IContainerRegistry RegisterMany(Type type, params Type[] serviceTypes) => throw _useServiceCollection;
  public IContainerRegistry RegisterScoped(Type from, Type to) => throw _useServiceCollection;
  public IContainerRegistry RegisterScoped(Type type, Func<object> factoryMethod) => throw _useServiceCollection;
  public IContainerRegistry RegisterScoped(Type type, Func<IContainerProvider, object> factoryMethod) => throw _useServiceCollection;


  public bool IsRegistered(Type type)
  {
    return _serviceProvider.GetService(type) is not null;
  }

  public bool IsRegistered(Type type, string name)
  {
    throw new NotSupportedException("Register with name is not supported.");
  }
}
