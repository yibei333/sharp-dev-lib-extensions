using System.Net.Sockets;

namespace SharpDevLib.Extensions.Transport.Tcp.Implemention
{
    internal class TcpListenerImp : SocketClient<TcpListenerOptions>, ITcpListener
    {
        public TcpListenerImp(Socket socket, TcpListenerOptions options) : base(socket, options, options.BufferSize)
        {
            Connections = new List<ITcpListenerConnection>();
        }

        public List<ITcpListenerConnection> Connections { get; }

        public event EventHandler<SocketArgs>? OnListening;
        public event EventHandler<SocketArgs>? OnAccept;
        public event EventHandler<SocketArgs>? OnDisConnected;
        public event EventHandler<SocketArgs>? OnError;
        public event EventHandler<SocketArgs>? OnReceive;
        public event EventHandler<SocketArgs>? OnSended;
        public event EventHandler<SocketArgs>? OnSendFailed;

        public void BeginLinsten()
        {
            try
            {
                Socket.Listen(Options.Backlog);
                OnListening?.Invoke(this, new SocketArgs { LocalEndPoint = LocalEndPoint, RemoteEndPoint = RemoteEndPoint });
                BeginAccept();
            }
            catch (Exception ex)
            {
                OnError?.Invoke(this, new SocketArgs { LocalEndPoint = LocalEndPoint, RemoteEndPoint = RemoteEndPoint, ErrorMessage = $"begin listen failed:{ex.Message}", Error = ex });
            }
        }

        private void BeginAccept()
        {
            try
            {
                Socket.BeginAccept(AcceptCallback, null);
            }
            catch (Exception ex)
            {
                OnError?.Invoke(this, new SocketArgs { LocalEndPoint = LocalEndPoint, RemoteEndPoint = RemoteEndPoint, ErrorMessage = $"begin accept failed:{ex.Message}", Error = ex });
            }
        }

        private void AcceptCallback(IAsyncResult result)
        {
            try
            {
                var socket = Socket.EndAccept(result);
                ITcpListenerConnection connection = new TcpListenerConnection(this, socket, Options);
                Connections.Add(connection);
                OnAccept?.Invoke(this, new SocketArgs { LocalEndPoint = socket.LocalEndPoint, RemoteEndPoint = socket.RemoteEndPoint });

                if (OnReceive.NotNull()) connection.OnReceive += OnReceive;
                if (OnSended.NotNull()) connection.OnSended += OnSended;
                if (OnSendFailed.NotNull()) connection.OnSendFailed += OnSendFailed;
                if (OnDisConnected.NotNull()) connection.OnDisConnected += OnDisConnected;
                if (OnError.NotNull()) connection.OnError += OnError;
                connection.BeginReceive();
            }
            catch (Exception ex)
            {
                OnError?.Invoke(this, new SocketArgs { LocalEndPoint = LocalEndPoint, RemoteEndPoint = RemoteEndPoint, ErrorMessage = $"accept callback failed:{ex.Message}", Error = ex });
            }
            BeginAccept();
        }

        /// <summary>
        /// send data to concrete remote
        /// </summary>
        /// <param name="connectionId">connection id</param>
        /// <param name="data">data</param>
        public void SendTo(Guid connectionId, byte[] data)
        {
            try
            {
                var connection = Connections.FirstOrDefault(x => x.Id == connectionId);
                if (connection.IsNull() || connection!.IsDead) throw new Exception("connection is closed");
                connection.Send(data);
            }
            catch (Exception ex)
            {
                OnError?.Invoke(this, new SocketArgs { LocalEndPoint = LocalEndPoint, RemoteEndPoint = RemoteEndPoint, ErrorMessage = $"send to connection[{connectionId}] failed:{ex.Message}", Error = ex, Data = data });
            }
        }

        /// <summary>
        /// send data to all connections
        /// </summary>
        /// <param name="data">data</param>
        public void SendToAll(byte[] data)
        {
            Connections.Where(x => !x.IsDead).ToList().ForEach(x => SendTo(x.Id, data));
        }

        public override void Dispose()
        {
            Close();
            Connections.ForEach(x => x.Dispose());
            Socket?.Dispose();
        }


        public override void Close()
        {
            Connections.ForEach(x => x.Close());
            Socket?.Close();
        }
    }
}
