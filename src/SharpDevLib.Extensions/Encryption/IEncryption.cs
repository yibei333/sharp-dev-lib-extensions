namespace SharpDevLib.Extensions.Encryption;

/// <summary>
/// encrption service
/// </summary>
public interface IEncryption
{
    /// <summary>
    /// get symmetric encryption service instance
    /// </summary>
    ISymmetricEncryption Symmetric { get; }
    /// <summary>
    /// get asymmetric encryption service instance
    /// </summary>
    IAsymmetricEncryption Asymmetric { get; }
}
