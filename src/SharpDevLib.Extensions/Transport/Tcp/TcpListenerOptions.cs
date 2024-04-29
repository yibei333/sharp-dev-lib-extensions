using System.Net;

namespace SharpDevLib.Extensions.Transport;

/// <summary>
/// tcp listner options
/// </summary>
public class TcpListenerOptions
{
    /// <summary>
    /// instantient tcp listner options
    /// </summary>
    /// <param name="local">local endpoint</param>
    /// <param name="bufferSize">data buffer size</param>
    public TcpListenerOptions(IPEndPoint local, int bufferSize = 4096)
    {
        Local = local;
        BufferSize = bufferSize;
    }

    /// <summary>
    /// instantient tcp listner options
    /// </summary>
    /// <param name="local">local endpoint</param>
    /// <param name="backlog">the maximum length of the pending connections queue.</param>
    /// <param name="bufferSize">data buffer size</param>
    public TcpListenerOptions(IPEndPoint local, int backlog, int bufferSize = 4096) : this(local, bufferSize)
    {
        Backlog = backlog;
    }

    /// <summary>
    /// local endpiont,this is required
    /// </summary>
    public IPEndPoint Local { get; }

    /// <summary>
    /// the maximum length of the pending connections queue.
    /// </summary>
    public int Backlog { get; }

    /// <summary>
    /// data buffer size
    /// </summary>
    public int BufferSize { get; }
}
