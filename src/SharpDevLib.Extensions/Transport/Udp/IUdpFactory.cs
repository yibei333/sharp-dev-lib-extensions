namespace SharpDevLib.Extensions.Transport;

/// <summary>
/// udp factory
/// </summary>
public interface IUdpFactory
{
    /// <summary>
    /// create client
    /// </summary>
    /// <param name="options">connetion options</param>
    /// <returns>udp client</returns>
    IUdpClient Create(UdpClientOptions options);
}