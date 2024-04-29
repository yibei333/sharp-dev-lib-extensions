using System.Net;
using System.Net.Sockets;

namespace SharpDevLib.Extensions.Transport;

internal abstract class SocketClient<TOptions> : ISocketClient<TOptions> where TOptions : class
{
    public SocketClient(Socket socket, TOptions options, int bufferSize)
    {
        if (bufferSize <= 0) throw new ArgumentOutOfRangeException(nameof(bufferSize));

        Id = Guid.NewGuid();
        Socket = socket;
        RemoteEndPoint = socket.RemoteEndPoint;
        LocalEndPoint = socket.LocalEndPoint;
        Options = options;
        Buffer = new byte[bufferSize];
        Block = new();
    }

    public Guid Id { get; }

    protected byte[] Buffer { get; }

    protected List<byte> Block { get; }

    public Socket Socket { get; }

    public TOptions Options { get; }

    public EndPoint? LocalEndPoint { get; internal set; }

    public EndPoint? RemoteEndPoint { get; internal set; }

    public abstract void Close();

    public abstract void Dispose();

    protected void EnsureConnected()
    {
        if (IsDisConnected)
        {
            Socket?.Close();
            throw new Exception("socket has been closed");
        }
    }

    protected bool IsDisConnected => Socket.IsNull() || !Socket.Connected;
}
