using System.Net;

namespace SharpDevLib.Extensions.Transport;

/// <summary>
/// udp client service
/// </summary>
public interface IUdpClient : ISocketClient<UdpClientOptions>
{
    /// <summary>
    /// begin receive
    /// </summary>
    void BeginReceive();

    /// <summary>
    /// send data to remote
    /// </summary>
    /// <param name="remoteEndPoint">remote endpoint</param>
    /// <param name="data">data to send</param>
    void Send(IPEndPoint remoteEndPoint, byte[] data);

    /// <summary>
    ///  when data received from remote,this method will be called
    /// </summary>
    event EventHandler<SocketArgs>? OnReceive;

    /// <summary>
    /// when send data success,this method will be called
    /// </summary>
    event EventHandler<SocketArgs>? OnSended;

    /// <summary>
    /// when send data failed,this method will be called
    /// </summary>
    event EventHandler<SocketArgs>? OnSendFailed;

    /// <summary>
    /// when exception occured,this method will be called
    /// </summary>
    event EventHandler<SocketArgs>? OnError;
}
