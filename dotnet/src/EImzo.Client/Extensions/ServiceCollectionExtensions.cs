using EImzo.Client;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for configuring and registering the E-IMZO Client SDK with <see cref="IServiceCollection"/>.
/// </summary>
public static class EImzoServiceCollectionExtensions
{
    /// <summary>
    /// Adds and configures the <see cref="IEImzoClient"/> typed client with <see cref="IHttpClientFactory"/>.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Optional configuration action for <see cref="EImzoClientOptions"/>.</param>
    /// <returns>An <see cref="IHttpClientBuilder"/> that can be used to further configure the client.</returns>
    public static IHttpClientBuilder AddEImzoClient(
        this IServiceCollection services,
        Action<EImzoClientOptions>? configure = null)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (configure != null)
        {
            services.Configure(configure);
        }
        else
        {
            services.AddOptions<EImzoClientOptions>();
        }

        return services.AddHttpClient<IEImzoClient, EImzoClient>((serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<EImzoClientOptions>>().Value;
            if (options.BaseUrl != null)
            {
                client.BaseAddress = options.BaseUrl;
            }

            if (options.Timeout > TimeSpan.Zero)
            {
                client.Timeout = options.Timeout;
            }
        });
    }

    /// <summary>
    /// Adds and configures the <see cref="IEImzoClient"/> typed client with a specified base URL.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="baseUrl">The base URL of the E-IMZO-SERVER.</param>
    /// <param name="defaultHost">Default site domain (Host header).</param>
    /// <returns>An <see cref="IHttpClientBuilder"/> that can be used to further configure the client.</returns>
    public static IHttpClientBuilder AddEImzoClient(
        this IServiceCollection services,
        Uri baseUrl,
        string? defaultHost = null)
    {
        return services.AddEImzoClient(options =>
        {
            options.BaseUrl = baseUrl;
            options.DefaultHost = defaultHost;
        });
    }
}
