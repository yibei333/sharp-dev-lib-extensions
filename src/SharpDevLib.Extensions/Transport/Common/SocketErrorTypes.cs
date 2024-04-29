namespace SharpDevLib.Extensions.Transport;

/// <summary>
/// socket error types
/// </summary>
public enum SocketErrorTypes
{
    /// <summary>
    /// connect error
    /// </summary>
    Connect,
    /// <summary>
    /// begin receive error
    /// </summary>
    BeginReceive,
    /// <summary>
    /// end receive error
    /// </summary>
    EndReceive,
    /// <summary>
    /// send error
    /// </summary>
    Send,
}
