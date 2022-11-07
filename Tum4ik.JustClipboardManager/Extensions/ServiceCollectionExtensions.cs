using System.Configuration;
using System.IO;
using System.Reflection;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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


  public static IServiceCollection AddDatabase(this IServiceCollection services)
  {
    return services.AddPooledDbContextFactory<AppDbContext>((sp, o) => AppDbContext.Configure(o), 2);
  }
}
