using System.Security.Cryptography.X509Certificates;

namespace SharpDevLib.Extensions.Certificate;

/// <summary>
/// certificate service
/// </summary>
public interface ICertificate
{
    /// <summary>
    /// generate ca cert
    /// </summary>
    /// <param name="option">ca option</param>
    /// <returns>cert</returns>
    X509Certificate2 GenerateCA(CACertOption option);

    /// <summary>
    /// generate client cert
    /// </summary>
    /// <param name="option">client option</param>
    /// <returns>cert</returns>
    X509Certificate2 GenerateClientCert(ClientCertOption option);

    /// <summary>
    /// generate server cert
    /// </summary>
    /// <param name="option">server option</param>
    /// <returns>cert</returns>
    X509Certificate2 GenerateServerCert(ServerCertOption option);
}
