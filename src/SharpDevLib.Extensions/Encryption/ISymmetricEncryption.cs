namespace SharpDevLib.Extensions.Encryption;

/// <summary>
/// symmetric encryptrion service
/// </summary>
public interface ISymmetricEncryption
{
    /// <summary>
    /// get aes encryption transform
    /// </summary>
    IEncryptionTransform<AesEncryptOption, AesDecryptOption> Aes { get; }

    /// <summary>
    /// get des encryption transform
    /// </summary>
    IEncryptionTransform<DesEncryptOption, DesDecryptOption> Des { get; }

    /// <summary>
    /// get 3des encryption transform
    /// </summary>
    IEncryptionTransform<TripleDesEncryptOption, TripleDesDecryptOption> TripleDes { get; }
}
