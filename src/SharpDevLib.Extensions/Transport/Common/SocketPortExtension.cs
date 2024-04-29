using System.Net;
using System.Net.NetworkInformation;

namespace SharpDevLib.Extensions.Transport;

/// <summary>
/// socket port extension
/// </summary>
public static class SocketPortExtension
{
    /// <summary>
    /// get a usable port by socket protocol type
    /// </summary>
    /// <param name="socketType">socket protocol type</param>
    /// <param name="min">min port</param>
    /// <param name="max">max port</param>
    /// <returns>usable port</returns>
    /// <exception cref="InvalidDataException"></exception>
    /// <exception cref="Exception"></exception>
    public static int GetUsablePort(this SocketProtocol socketType, int min = IPEndPoint.MinPort, int max = IPEndPoint.MaxPort)
    {
        if (min <= IPEndPoint.MinPort || min > IPEndPoint.MaxPort) throw new InvalidDataException($"min port range {IPEndPoint.MinPort}-{IPEndPoint.MaxPort}");
        if (max <= IPEndPoint.MinPort || max > IPEndPoint.MaxPort) throw new InvalidDataException($"max port range {IPEndPoint.MinPort}-{IPEndPoint.MaxPort}");
        if (min > max) throw new InvalidDataException($"min port can not greator than max port");
        var ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
        var endPoints = (socketType == SocketProtocol.TCP ? ipGlobalProperties.GetActiveTcpListeners() : ipGlobalProperties.GetActiveUdpListeners()).Where(x => x.Port >= min && x.Port <= max).ToList();
        var xx = ipGlobalProperties.GetActiveTcpConnections();
        for (int i = min; i <= max; i++)
        {
            if (endPoints.All(x => x.Port != i)) return i;
        }
        throw new Exception($"no usable port detected");
    }
}
