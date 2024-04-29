using System.Net.Sockets;

namespace SharpDevLib.Extensions.Transport.Tcp.Implemention;

internal class TcpListenerConnection : SocketClient<TcpListenerOptions>, ITcpListenerConnection
{
    public TcpListenerConnection(ITcpListener listener, Socket socket, TcpListenerOptions options) : base(socket, options, options.BufferSize)
    {
        Listener = listener;
    }

    public ITcpListener Listener { get; }

    public bool IsDead { get; private set; }

    public event EventHandler<SocketArgs>? OnDisConnected;
    public event EventHandler<SocketArgs>? OnReceive;
    public event EventHandler<SocketArgs>? OnSended;
    public event EventHandler<SocketArgs>? OnSendFailed;
    public event EventHandler<SocketArgs>? OnError;

    public void BeginReceive()
    {
        try
        {
            EnsureConnected();
            Socket.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, ReceiveCallback, null);
        }
        catch (Exception ex)
        {
            OnError?.Invoke(this, new SocketArgs { LocalEndPoint = LocalEndPoint, RemoteEndPoint = RemoteEndPoint, ErrorMessage = $"receive failed:{ex.Message}", Error = ex });
        }
    }

    private void ReceiveCallback(IAsyncResult result)
    {
        try
        {
            EnsureConnected();
            var length = Socket.EndReceive(result);
            Block.AddRange(Buffer.Take(length));
            if (Socket.Available == 0)
            {
                OnReceive?.Invoke(this, new SocketArgs { LocalEndPoint = LocalEndPoint, RemoteEndPoint = RemoteEndPoint, Data = Block.ToArray() });
                Block.Clear();
            }
        }
        catch (Exception ex)
        {
            OnError?.Invoke(this, new SocketArgs { LocalEndPoint = LocalEndPoint, RemoteEndPoint = RemoteEndPoint, ErrorMessage = $"receive callback failed:{ex.Message}", Error = ex });
        }
        BeginReceive();
    }

    public void Send(byte[] data)
    {
        try
        {
            EnsureConnected();
            Socket.Send(data);
            OnSended?.Invoke(this, new SocketArgs { LocalEndPoint = LocalEndPoint, RemoteEndPoint = RemoteEndPoint, Data = data });
        }
        catch (Exception ex)
        {
            OnSendFailed?.Invoke(this, new SocketArgs { LocalEndPoint = LocalEndPoint, RemoteEndPoint = RemoteEndPoint, ErrorType = SocketErrorTypes.Send, ErrorMessage = ex.Message, Error = ex, Data = data });
            OnError?.Invoke(this, new SocketArgs { LocalEndPoint = LocalEndPoint, RemoteEndPoint = RemoteEndPoint, ErrorMessage = $"send data failed:{ex.Message}", Error = ex, Data = data });
        }
    }

    public override void Close()
    {
        Socket?.Close();
        IsDead = true;
        OnDisConnected?.Invoke(Listener, new SocketArgs { LocalEndPoint = LocalEndPoint, RemoteEndPoint = RemoteEndPoint });
    }

    public override void Dispose()
    {
        Close();
        Socket?.Dispose();
    }
}