namespace SharpDevLib.Extensions.Transport;

/// <summary>
/// tcp listener connection
/// </summary>
public interface ITcpListenerConnection : ISocketClient<TcpListenerOptions>
{
    /// <summary>
    /// listener
    /// </summary>
    ITcpListener Listener { get; }

    /// <summary>
    /// is dead connection
    /// </summary>
    bool IsDead { get; }

    /// <summary>
    /// begin receive
    /// </summary>
    internal void BeginReceive();

    /// <summary>
    /// when client disconnected,this method will be called
    /// </summary>
    internal event EventHandler<SocketArgs>? OnDisConnected;

    /// <summary>
    /// when exception occured,this method will be called
    /// </summary>
    internal event EventHandler<SocketArgs>? OnError;

    /// <summary>
    /// when received data,this method will be called
    /// </summary>
    internal event EventHandler<SocketArgs>? OnReceive;

    /// <summary>
    /// when send data success,this method will be called
    /// </summary>
    internal event EventHandler<SocketArgs>? OnSended;

    /// <summary>
    /// when send data failed,this method will be called
    /// </summary>
    internal event EventHandler<SocketArgs>? OnSendFailed;

    /// <summary>
    /// send data to server
    /// </summary>
    /// <param name="data">data to send</param>
    internal void Send(byte[] data);
}