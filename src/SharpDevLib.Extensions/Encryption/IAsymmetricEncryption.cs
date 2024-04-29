namespace SharpDevLib.Extensions.Encryption;

/// <summary>
/// asymmetric encryption service
/// </summary>
public interface IAsymmetricEncryption
{
    /// <summary>
    /// get rsa encryption transform
    /// </summary>
    IRsaEncryptionTransform Rsa { get; }
}