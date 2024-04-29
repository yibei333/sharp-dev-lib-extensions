namespace SharpDevLib.Extensions.Transport;

/// <summary>
/// tcp factory
/// </summary>
public interface ITcpFactory
{
    /// <summary>
    /// create client
    /// </summary>
    /// <param name="options">connetion options</param>
    /// <returns>tcp client</returns>
    ITcpClient Create(TcpClientOptions options);

    /// <summary>
    /// create listener
    /// </summary>
    /// <param name="options">connetion options</param>
    /// <returns>tcp listener</returns>
    ITcpListener Create(TcpListenerOptions options);
}