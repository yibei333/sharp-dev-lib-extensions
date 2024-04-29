using System.Net;
using System.Net.Sockets;

namespace SharpDevLib.Extensions.Transport.Udp.Implemention;

internal class UdpClientImp : SocketClient<UdpClientOptions>, IUdpClient
{
    public UdpClientImp(Socket socket, UdpClientOptions options) : base(socket, options, options.BufferSize ?? 4096)
    {
        if (options.BufferSize > 4096) throw new ArgumentOutOfRangeException(nameof(options.BufferSize), $"udp packet length can't greator than 4096,reduce it please");
    }

    public event EventHandler<SocketArgs>? OnError;
    public event EventHandler<SocketArgs>? OnReceive;
    public event EventHandler<SocketArgs>? OnSended;
    public event EventHandler<SocketArgs>? OnSendFailed;

    public void BeginReceive()
    {
        try
        {
            EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
            Socket.BeginReceiveFrom(Buffer, 0, Buffer.Length, SocketFlags.None, ref remoteEndPoint, ReceiveCallback, null);
        }
        catch (Exception ex)
        {
            OnError?.Invoke(this, new SocketArgs { Socket = Socket, ErrorType = SocketErrorTypes.BeginReceive, ErrorMessage = ex.Message, Error = ex, LocalEndPoint = Options.Local });
        }
    }

    private void ReceiveCallback(IAsyncResult result)
    {
        try
        {
            EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
            var length = Socket.EndReceiveFrom(result, ref remoteEndPoint);
            Block.AddRange(Buffer.Take(length));
            if (Socket.Available == 0)
            {
                OnReceive?.Invoke(this, new SocketArgs { Socket = Socket, Data = Block.ToArray(), LocalEndPoint = Options.Local, RemoteEndPoint = remoteEndPoint });
                Block.Clear();
            }
        }
        catch (Exception ex)
        {
            OnError?.Invoke(this, new SocketArgs { Socket = Socket, ErrorType = SocketErrorTypes.EndReceive, ErrorMessage = ex.Message, Error = ex, LocalEndPoint = Options.Local });
        }
        BeginReceive();
    }

    public override void Close()
    {
        Socket?.Close();
    }

    public override void Dispose()
    {
        Close();
        Socket?.Dispose();
    }

    public void Send(IPEndPoint remoteEndPoint, byte[] data)
    {
        try
        {
            if (data.Length > 4096) throw new ArgumentOutOfRangeException(nameof(data), $"udp packet length can't greator than 4096,reduce it please");
            Socket.SendTo(data, remoteEndPoint);
            OnSended?.Invoke(this, new SocketArgs { Socket = Socket, Data = data, LocalEndPoint = Options.Local, RemoteEndPoint = remoteEndPoint });
        }
        catch (Exception ex)
        {
            OnSendFailed?.Invoke(this, new SocketArgs { Socket = Socket, Data = data, ErrorType = SocketErrorTypes.Send, ErrorMessage = ex.Message, Error = ex, LocalEndPoint = Options.Local, RemoteEndPoint = remoteEndPoint });
            OnError?.Invoke(this, new SocketArgs { Socket = Socket, Data = data, ErrorType = SocketErrorTypes.Send, ErrorMessage = ex.Message, Error = ex, LocalEndPoint = Options.Local, RemoteEndPoint = remoteEndPoint });
        }
    }
}
