namespace SharpDevLib.Extensions.Transport;

/// <summary>
/// tcp client service
/// </summary>
public interface ITcpClient : ISocketClient<TcpClientOptions>
{
    /// <summary>
    /// conntct and begin receive
    /// </summary>
    void BeginConnect();

    /// <summary>
    /// send data to server
    /// </summary>
    /// <param name="data">data to send</param>
    void Send(byte[] data);

    /// <summary>
    /// when connection connected,this method will be called
    /// </summary>
    event EventHandler<SocketArgs>? OnConnected;

    /// <summary>
    /// when received data from remote,this method will be called
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
    /// when connection disconnected,this method will be called
    /// </summary>
    event EventHandler<SocketArgs>? OnDisConnected;

    /// <summary>
    /// when exception occured,this method will be called
    /// </summary>
    event EventHandler<SocketArgs>? OnError;
}
