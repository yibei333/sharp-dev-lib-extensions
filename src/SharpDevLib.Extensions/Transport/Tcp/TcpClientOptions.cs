using System.Net;

namespace SharpDevLib.Extensions.Transport;

/// <summary>
/// tcp client options
/// </summary>
public class TcpClientOptions
{
    /// <summary>
    /// instantient tcp client options
    /// </summary>
    /// <param name="remote">remote endpoint</param>
    public TcpClientOptions(IPEndPoint remote)
    {
        Remote = remote;
    }

    /// <summary>
    /// instantient tcp client options
    /// </summary>
    /// <param name="local">local endpoint</param>
    /// <param name="remote">remote endpoint</param>
    public TcpClientOptions(IPEndPoint? local, IPEndPoint remote) : this(remote)
    {
        Local = local;
    }

    /// <summary>
    /// instantient tcp client options
    /// </summary>
    /// <param name="remote">remote endpoint</param>
    /// <param name="bufferSize">buffer size</param>
    public TcpClientOptions(IPEndPoint remote, int? bufferSize) : this(remote)
    {
        BufferSize = bufferSize;
    }

    /// <summary>
    /// instantient tcp client options
    /// </summary>
    /// <param name="local">local endpoint</param>
    /// <param name="remote">remote endpoint</param>
    /// <param name="bufferSize">buffer size</param>
    public TcpClientOptions(IPEndPoint? local, IPEndPoint remote, int? bufferSize) : this(local, remote)
    {
        BufferSize = bufferSize;
    }

    /// <summary>
    /// local endpoint,if null,local will be dynamic
    /// </summary>
    public IPEndPoint? Local { get; }

    /// <summary>
    /// remote endpoint,this is required
    /// </summary>
    public IPEndPoint Remote { get; }

    /// <summary>
    /// buffer size
    /// </summary>
    public int? BufferSize { get; }
}
