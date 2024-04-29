namespace SharpDevLib.Extensions.Transport.Tcp.Implemention;

internal class TcpClientImp : SocketClient<TcpClientOptions>, ITcpClient
{
    public TcpClientImp(System.Net.Sockets.Socket socket, TcpClientOptions options) : base(socket, options, options.BufferSize ?? 4096)
    {
    }

    public event EventHandler<SocketArgs>? OnConnected;
    public event EventHandler<SocketArgs>? OnReceive;
    public event EventHandler<SocketArgs>? OnSended;
    public event EventHandler<SocketArgs>? OnSendFailed;
    public event EventHandler<SocketArgs>? OnDisConnected;
    public event EventHandler<SocketArgs>? OnError;

    public void BeginConnect()
    {
        try
        {
            if (Options.Remote.IsNull()) throw new NullReferenceException();
            Socket.Connect(Options.Remote);
            LocalEndPoint = Socket.LocalEndPoint;
            RemoteEndPoint = Socket.RemoteEndPoint;
            OnConnected?.Invoke(this, new SocketArgs { LocalEndPoint = LocalEndPoint, RemoteEndPoint = RemoteEndPoint });
            BeginReceive();
        }
        catch (Exception ex)
        {
            OnError?.Invoke(this, new SocketArgs { LocalEndPoint = LocalEndPoint, RemoteEndPoint = RemoteEndPoint, ErrorType = SocketErrorTypes.Connect, ErrorMessage = ex.Message, Error = ex });
        }
    }

    private void BeginReceive()
    {
        try
        {
            EnsureConnected();
            Socket.BeginReceive(Buffer, 0, Buffer.Length, System.Net.Sockets.SocketFlags.None, ReceiveCallback, null);
        }
        catch (Exception ex)
        {
            OnError?.Invoke(this, new SocketArgs { LocalEndPoint = LocalEndPoint, RemoteEndPoint = RemoteEndPoint, ErrorType = SocketErrorTypes.BeginReceive, ErrorMessage = ex.Message, Error = ex });
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
            OnError?.Invoke(this, new SocketArgs { LocalEndPoint = LocalEndPoint, RemoteEndPoint = RemoteEndPoint, ErrorType = SocketErrorTypes.EndReceive, ErrorMessage = ex.Message, Error = ex });
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
            OnError?.Invoke(this, new SocketArgs { LocalEndPoint = LocalEndPoint, RemoteEndPoint = RemoteEndPoint, ErrorType = SocketErrorTypes.Send, ErrorMessage = ex.Message, Error = ex, Data = data });
        }
    }

    public override void Close()
    {
        Socket?.Close();
        OnDisConnected?.Invoke(this, new SocketArgs { LocalEndPoint = LocalEndPoint, RemoteEndPoint = RemoteEndPoint });
    }

    public override void Dispose()
    {
        Close();
        Socket?.Dispose();
    }
}
