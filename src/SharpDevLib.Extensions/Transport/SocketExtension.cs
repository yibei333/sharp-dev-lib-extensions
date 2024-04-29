using Microsoft.Extensions.DependencyInjection;

namespace SharpDevLib.Extensions.Transport;

/// <summary>
/// socket di extension
/// </summary>
public static class SocketExtension
{
    /// <summary>
    /// add socket services
    /// </summary>
    public static IServiceCollection AddSocket(this IServiceCollection services)
    {
        services.AddSingleton<ITcpFactory, TcpFactory>();
        services.AddSingleton<IUdpFactory, UdpFactory>();
        return services;
    }
}
