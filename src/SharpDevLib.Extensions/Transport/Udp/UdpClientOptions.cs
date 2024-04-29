using System.Net;

namespace SharpDevLib.Extensions.Transport;

/// <summary>
/// udp client options
/// </summary>
public class UdpClientOptions
{
    /// <summary>
    /// instantient upd client options
    /// </summary>
    public UdpClientOptions()
    {

    }

    /// <summary>
    /// instantient upd client options
    /// </summary>
    /// <param name="local">local endpoint</param>
    public UdpClientOptions(IPEndPoint local)
    {
        Local = local;
    }

    /// <summary>
    /// instantient upd client options
    /// </summary>
    /// <param name="bufferSize">buffer size</param>
    public UdpClientOptions(int bufferSize)
    {
        BufferSize = bufferSize;
    }

    /// <summary>
    /// instantient upd client options
    /// </summary>
    /// <param name="local">local endpoint</param>
    /// <param name="bufferSize">buffer size</param>
    public UdpClientOptions(IPEndPoint local, int bufferSize) : this(local)
    {
        BufferSize = bufferSize;
    }

    /// <summary>
    /// local endpoint,if null,local will be dynamic
    /// </summary>
    public IPEndPoint? Local { get; internal set; }

    /// <summary>
    /// buffer size
    /// </summary>
    public int? BufferSize { get; }
}
