using System.Reflection;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Prism.Events;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using Tum4ik.JustClipboardManager.Data;

namespace Tum4ik.JustClipboardManager.Extensions;
internal static class ServiceCollectionExtensions
{
  /// <summary>
  /// Registers view with its view model.
  /// </summary>
  public static IServiceCollection RegisterView<TView, TViewModel>(
    this IServiceCollection services,
    ServiceLifetime viewLifetime = ServiceLifetime.Transient
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


  public static IServiceCollection RegisterForNavigation<TView, TViewModel>(this IServiceCollection services)
    where TView : class
  {
    ViewModelLocationProvider.Register<TView, TViewModel>();
    return services.AddTransient<TView>();
  }


  public static IServiceCollection AddConfiguration(this IServiceCollection services)
  {
    var assembly = Assembly.GetExecutingAssembly();
    var appsettingsResourceName = assembly.GetManifestResourceNames()
      .Single(n => n.EndsWith("appsettings.json", StringComparison.InvariantCultureIgnoreCase));
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
      .AddSingleton<IDialogService, DialogService>()
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
      .AddTransient<IDialogWindow, DialogWindow>();
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
