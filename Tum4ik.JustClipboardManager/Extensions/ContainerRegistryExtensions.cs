using System.Reflection;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Prism.Ioc;
using Prism.Services.Dialogs;
using Tum4ik.JustClipboardManager.Data;
using Tum4ik.JustClipboardManager.Ioc.Wrappers;
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


  public static IContainerRegistry RegisterConfiguration(this IContainerRegistry services)
  {
    var assembly = Assembly.GetExecutingAssembly();
    var appsettingsResourceName = assembly.GetManifestResourceNames()
      .Single(n => n.EndsWith("appsettings.json", StringComparison.OrdinalIgnoreCase));
    // Don't use "using" keyword for appsettingsStream here - it will break the settings reading process.
    // The stream will be disposed by StreamReader internally anyway.
    var appsettingsStream = assembly.GetManifestResourceStream(appsettingsResourceName);
    var configuration = new ConfigurationBuilder()
      .SetBasePath(AppContext.BaseDirectory)
      .AddJsonStream(appsettingsStream!)
#if DEBUG
      .AddUserSecrets(Assembly.GetExecutingAssembly())
#endif
      .Build();
    return services.RegisterInstance<IConfiguration>(configuration);
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


  public static IContainerRegistry RegisterGeneratedWrappers(this IContainerRegistry services)
  {
    return services
      .Register<IProcess, ProcessWrapper>()
      .RegisterSingleton<IEnvironment, EnvironmentWrapper>()
      .RegisterSingleton<IFile, FileWrapper>()
      .RegisterSingleton<IPath, PathWrapper>()
      .RegisterSingleton<IClipboard, ClipboardWrapper>();
  }
}
