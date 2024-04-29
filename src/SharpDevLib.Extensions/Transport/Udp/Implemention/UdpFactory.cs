using SharpDevLib.Extensions.Transport.Udp.Implemention;
using System.Net;
using System.Net.Sockets;

namespace SharpDevLib.Extensions.Transport;

/// <summary>
/// UdpFactory
/// </summary>
public class UdpFactory : IUdpFactory
{
    /// <summary>
    /// create udp client
    /// </summary>
    /// <param name="options">udp client options</param>
    /// <returns></returns>
    public IUdpClient Create(UdpClientOptions options)
    {
        if (options.IsNull()) throw new ArgumentNullException(nameof(options));
        var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        options.Local = options.Local.NotNull() ? options.Local! : new IPEndPoint(IPAddress.Any, SocketProtocol.UDP.GetUsablePort(10000));
        socket.Bind(options.Local);
        return new UdpClientImp(socket, options);
    }
}