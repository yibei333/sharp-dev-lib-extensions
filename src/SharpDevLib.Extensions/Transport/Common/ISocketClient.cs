using System.Net;
using System.Net.Sockets;

namespace SharpDevLib.Extensions.Transport;

/// <summary>
/// socket client instance
/// </summary>
public interface ISocketClient<TOptions> : IDisposable where TOptions : class
{
    /// <summary>
    /// local endpoint
    /// </summary>
    EndPoint? LocalEndPoint { get; }
    /// <summary>
    /// remote endpoint
    /// </summary>
    EndPoint? RemoteEndPoint { get; }
    /// <summary>
    /// client id
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// socket instance
    /// </summary>
    Socket Socket { get; }

    /// <summary>
    /// socket options
    /// </summary>
    TOptions Options { get; }

    /// <summary>
    /// close connection
    /// </summary>
    void Close();
}
