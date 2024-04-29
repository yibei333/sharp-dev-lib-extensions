namespace SharpDevLib.Extensions.Transport;

/// <summary>
/// tcp listener service
/// </summary>
public interface ITcpListener : ISocketClient<TcpListenerOptions>
{
    /// <summary>
    /// begin listen and accept connections
    /// </summary>
    void BeginLinsten();

    /// <summary>
    /// when start listening,this method will be called
    /// </summary>
    event EventHandler<SocketArgs>? OnListening;

    /// <summary>
    /// when accept a connection,this method will be called
    /// </summary>
    event EventHandler<SocketArgs>? OnAccept;

    /// <summary>
    /// when client disconnected,this method will be called
    /// </summary>
    event EventHandler<SocketArgs>? OnDisConnected;

    /// <summary>
    /// when exception occured,this method will be called
    /// </summary>
    event EventHandler<SocketArgs>? OnError;

    /// <summary>
    /// when received data,this method will be called
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
    /// tcp connections
    /// </summary>
    List<ITcpListenerConnection> Connections { get; }

    /// <summary>
    /// send data to concrete remote
    /// </summary>
    /// <param name="connectionId">connection id</param>
    /// <param name="data">data</param>
    void SendTo(Guid connectionId, byte[] data);

    /// <summary>
    /// send data to all connections
    /// </summary>
    /// <param name="data">data</param>
    void SendToAll(byte[] data);
}
