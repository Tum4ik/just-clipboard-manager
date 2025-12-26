using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Threading;
using Prism.Ioc;
using Prism.Services.Dialogs;
using Tum4ik.JustClipboardManager.Data;
using Tum4ik.JustClipboardManager.Services.Dialogs;

namespace Tum4ik.JustClipboardManager.Extensions;
internal static class ContainerRegistryExtensions
{
  /// <summary>
  /// Registers the shell element (for example <see cref="Window"/>) with its view model.
  /// </summary>
  /// <typeparam name="TView"></typeparam>
  /// <typeparam name="TViewModel"></typeparam>
  /// <param name="services"></param>
  /// <param name="viewLifetime"></param>
  /// <returns>The <see cref="IContainerRegistry"/>.</returns>
  public static IContainerRegistry RegisterShell<TView, TViewModel>(
    this IContainerRegistry services
  )
    where TView : FrameworkElement, new()
    where TViewModel : notnull
  {
    services.Register<TViewModel>();
    services.RegisterSingleton<TView>(cp =>
    {
      var viewModel = cp.Resolve<TViewModel>();
      var view = new TView { DataContext = viewModel };
      return view;
    });
    return services;
  }


  public static void RegisterSingleInstanceDialog<TView, TViewModel>(this IContainerRegistry services, string name)
    where TView : FrameworkElement
    where TViewModel : class, IDialogAware
  {
    SingleInstanceDialogsProvider.RegisterSingleInstanceDialog(name);
    services.RegisterDialog<TView, TViewModel>(name);
  }


  public static IContainerRegistry RegisterDatabase(this IContainerRegistry containerRegistry)
  {
    var services = new ServiceCollection();
    services.AddPooledDbContextFactory<AppDbContext>((sp, o) => AppDbContext.Configure(o), 2);
    var serviceProvider = services.BuildServiceProvider();

    return containerRegistry.RegisterSingleton<IDbContextFactory<AppDbContext>>(
      serviceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>
    );
  }



  public static IContainerRegistry RegisterThreadSwitching(this IContainerRegistry services)
  {
    var mainThread = Thread.CurrentThread;
    var synchronizationContext = SynchronizationContext.Current;
    return services
      .RegisterSingleton<JoinableTaskContext>(() => new JoinableTaskContext(mainThread, synchronizationContext))
      .RegisterSingleton<JoinableTaskFactory>();
  }
}
