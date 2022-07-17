using System.Windows;
using Microsoft.Extensions.DependencyInjection;

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
      return new TView
      {
        DataContext = viewModel
      };
    }, viewLifetime);
    services.Add(viewModelDescriptor);
    services.Add(viewDescriptor);
    return services;
  }
}
