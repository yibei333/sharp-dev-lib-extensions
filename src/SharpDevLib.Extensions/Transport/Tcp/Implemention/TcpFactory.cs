using SharpDevLib.Extensions.Transport.Tcp.Implemention;
using System.Net.Sockets;

namespace SharpDevLib.Extensions.Transport;

/// <summary>
/// tcp factory
/// </summary>
public class TcpFactory : ITcpFactory
{
    /// <summary>
    /// create client
    /// </summary>
    /// <param name="options">connetion options</param>
    /// <returns>tcp client</returns>
    public ITcpClient Create(TcpClientOptions options)
    {
        if (options.IsNull()) throw new ArgumentNullException(nameof(options));
        if (options.Remote.IsNull()) throw new NullReferenceException(nameof(options.Remote));

        var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        if (options.Local.NotNull()) socket.Bind(options.Local!);
        return new TcpClientImp(socket, options);
    }

    /// <summary>
    /// create listener
    /// </summary>
    /// <param name="options">connetion options</param>
    /// <returns>tcp listener</returns>
    public ITcpListener Create(TcpListenerOptions options)
    {
        if (options.IsNull()) throw new ArgumentNullException(nameof(options));
        if (options.Local.IsNull()) throw new NullReferenceException(nameof(options.Local));
        var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.Bind(options.Local);
        return new TcpListenerImp(socket, options);
    }
}
