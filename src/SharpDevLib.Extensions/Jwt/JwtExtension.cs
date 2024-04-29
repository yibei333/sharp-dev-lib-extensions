using Microsoft.Extensions.DependencyInjection;

namespace SharpDevLib.Extensions.Jwt;

/// <summary>
/// json web token service di register extension
/// </summary>
public static class JwtExtension
{
    /// <summary>
    /// add json web token service
    /// </summary>
    /// <param name="services">service collection</param>
    /// <returns>service collection</returns>
    public static IServiceCollection AddJwt(this IServiceCollection services)
    {
        services.AddTransient<IJwtService, JwtService>();
        return services;
    }
}
