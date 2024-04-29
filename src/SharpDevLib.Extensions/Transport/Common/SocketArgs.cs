using System.Net;
using System.Net.Sockets;

namespace SharpDevLib.Extensions.Transport;

/// <summary>
/// socket event args
/// </summary>
public class SocketArgs
{
    /// <summary>
    /// remote end point
    /// </summary>
    public EndPoint? RemoteEndPoint { get; internal set; }

    /// <summary>
    /// local end point
    /// </summary>
    public EndPoint? LocalEndPoint { get; internal set; }

    /// <summary>
    /// data
    /// </summary>
    public byte[]? Data { get; internal set; }

    /// <summary>
    /// exception
    /// </summary>
    public Exception? Error { get; internal set; }

    /// <summary>
    /// error message
    /// </summary>
    public string? ErrorMessage { get; internal set; }

    /// <summary>
    /// socket instance
    /// </summary>
    public Socket? Socket { get; internal set; }

    /// <summary>
    /// error type
    /// </summary>
    public SocketErrorTypes? ErrorType { get; internal set; }
}
