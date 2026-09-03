using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace JustClipboardManager.Infrastructure.Extensions;

internal static class ServiceCollectionExtensions
{
  extension(IServiceCollection services)
  {
    public OptionsBuilder<TOptions> ConfigureWithValidateOnStart<TOptions>(IConfigurationSection configurationSection)
    where TOptions : class
    {
      return services
        .AddOptionsWithValidateOnStart<TOptions>()
        .Bind(configurationSection)
        .ValidateDataAnnotations();
    }
  }
}
