using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Prism.Events;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Regions.Behaviors;
using Prism.Services.Dialogs;
using Tum4ik.JustClipboardManager.Data;
using Tum4ik.JustClipboardManager.Ioc;
using Tum4ik.JustClipboardManager.Services;
using Tum4ik.JustClipboardManager.Views.Main;

namespace Tum4ik.JustClipboardManager.Extensions;
internal static class ServiceCollectionExtensions
{
  /// <summary>
  /// Registers the shell element (for example <see cref="Window"/>) with its view model.
  /// </summary>
  /// <typeparam name="TView"></typeparam>
  /// <typeparam name="TViewModel"></typeparam>
  /// <param name="services"></param>
  /// <param name="viewLifetime"></param>
  /// <returns>The <see cref="IServiceCollection"/>.</returns>
  public static IServiceCollection RegisterShell<TView, TViewModel>(
    this IServiceCollection services,
    ServiceLifetime viewLifetime
  )
    where TView : FrameworkElement, new()
    where TViewModel : notnull
  {
    var viewModelDescriptor = new ServiceDescriptor(typeof(TViewModel), typeof(TViewModel), ServiceLifetime.Transient);
    var viewDescriptor = new ServiceDescriptor(typeof(TView), serviceProvider =>
    {
      var viewModel = serviceProvider.GetRequiredService<TViewModel>();
      var view = new TView { DataContext = viewModel };
      return view;
    }, viewLifetime);
    services.Add(viewModelDescriptor);
    services.Add(viewDescriptor);
    return services;
  }


  /// <summary>
  /// Registers a <see cref="ContentControl"/> object for navigation to be used in the <see cref="IRegionManager"/>.
  /// </summary>
  /// <typeparam name="TView">The Type of the ContentControl object to register as the view.</typeparam>
  /// <typeparam name="TViewModel">The view model to be used as the DataContext for the view.</typeparam>
  /// <param name="services">The services collection.</param>
  /// <param name="name">The unique name to register with the view.</param>
  /// <returns>The <see cref="IServiceCollection"/>.</returns>
  public static IServiceCollection RegisterForNavigation<TView, TViewModel>(this IServiceCollection services,
                                                                            string name)
    where TView : FrameworkElement
    where TViewModel : class
  {
    ViewModelLocationProvider.Register<TView, TViewModel>();
    ServiceKeyToTypeMappings.Add(name, typeof(TView));
    return services
      .AddTransient<TView>()
      .AddTransient<TViewModel>();
  }


  /// <summary>
  /// Registers a <see cref="ContentControl"/> object to be used as a dialog in the <see cref="IDialogService"/>.
  /// </summary>
  /// <typeparam name="TView">The Type of the ContentControl object to register as the dialog.</typeparam>
  /// <typeparam name="TViewModel">The view model to be used as the DataContext for the dialog.</typeparam>
  /// <param name="services">The services collection.</param>
  /// <param name="name">The unique name to register with the dialog.</param>
  /// <returns>The <see cref="IServiceCollection"/>.</returns>
  public static IServiceCollection RegisterDialog<TView, TViewModel>(this IServiceCollection services, string name)
    where TView : FrameworkElement
    where TViewModel : class, IDialogAware
  {
    return services.RegisterForNavigation<TView, TViewModel>(name);
  }


  public static IServiceCollection RegisterSingleInstanceDialog<TView, TViewModel>(this IServiceCollection services,
                                                                                   string name)
    where TView : FrameworkElement
    where TViewModel : class, IDialogAware
  {
    SingleInstanceDialogsProvider.RegisterSingleInstanceDialog(name);
    return services.RegisterDialog<TView, TViewModel>(name);
  }


  public static IServiceCollection AddConfiguration(this IServiceCollection services)
  {
    var assembly = Assembly.GetExecutingAssembly();
    var appsettingsResourceName = assembly.GetManifestResourceNames()
      .Single(n => n.EndsWith("appsettings.json", StringComparison.OrdinalIgnoreCase));
    // Don't use "using" keyword for appsettingsStream here - it will break the settings reading process.
    // The stream will be disposed by StreamReader internally anyway.
    var appsettingsStream = assembly.GetManifestResourceStream(appsettingsResourceName);
    var configuration = new ConfigurationBuilder()
      .SetBasePath(AppContext.BaseDirectory)
      .AddJsonStream(appsettingsStream)
#if DEBUG
      .AddUserSecrets(Assembly.GetExecutingAssembly())
#endif
      .Build();
    return services.AddSingleton<IConfiguration>(configuration);
  }


  public static IServiceCollection AddPrismServices(this IServiceCollection services)
  {
    return services
      .AddSingleton<IDialogService, ExtendedDialogService>()
      .AddSingleton<IModuleInitializer, ModuleInitializer>()
      .AddSingleton<IModuleManager, ModuleManager>()
      .AddSingleton<RegionAdapterMappings>()
      .AddSingleton<IRegionManager, RegionManager>()
      .AddSingleton<IRegionNavigationContentLoader, RegionNavigationContentLoader>()
      .AddSingleton<IEventAggregator, EventAggregator>()
      .AddSingleton<IRegionViewRegistry, RegionViewRegistry>()
      .AddSingleton<IRegionBehaviorFactory, RegionBehaviorFactory>()
      .AddTransient<IRegionNavigationJournalEntry, RegionNavigationJournalEntry>()
      .AddTransient<IRegionNavigationJournal, RegionNavigationJournal>()
      .AddTransient<IRegionNavigationService, RegionNavigationService>()
      .AddTransient<IDialogWindow, MainDialogWindow>();
  }


  public static IServiceCollection AddPrismBehaviors(this IServiceCollection services)
  {
    return services
      .AddTransient<DelayedRegionCreationBehavior>()
      .AddTransient<BindRegionContextToDependencyObjectBehavior>()
      .AddTransient<RegionActiveAwareBehavior>()
      .AddTransient<SyncRegionContextWithHostBehavior>()
      .AddTransient<RegionManagerRegistrationBehavior>()
      .AddTransient<RegionMemberLifetimeBehavior>()
      .AddTransient<ClearChildViewsRegionBehavior>()
      .AddTransient<AutoPopulateRegionBehavior>()
      .AddTransient<DestructibleRegionBehavior>();
  }


  public static IServiceCollection AddRegionAdapters(this IServiceCollection services)
  {
    return services
      .AddTransient<SelectorRegionAdapter>()
      .AddTransient<ItemsControlRegionAdapter>()
      .AddTransient<ContentControlRegionAdapter>();
  }


  public static IServiceCollection AddDatabase(this IServiceCollection services)
  {
    return services.AddPooledDbContextFactory<AppDbContext>((sp, o) => AppDbContext.Configure(o), 2);
  }
}
