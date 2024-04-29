using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SharpDevLib.Extensions.Http;

/// <summary>
/// http di extension
/// </summary>
public static class HttpExtension
{
    /// <summary>
    /// add http service
    /// </summary>
    /// <param name="services">service collection</param>
    /// <param name="configuration">configuration</param>
    /// <returns>service collection</returns>
    public static IServiceCollection AddHttp(this IServiceCollection services, IConfiguration? configuration = null)
    {
        if (configuration.NotNull())
        {
            services.Configure<HttpGlobalOptions>(configuration!.GetSection("HttpService"));
        }
        services.AddTransient<IHttpService, HttpService>();
        return services;
    }
}