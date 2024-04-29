using System.Security.Cryptography;

namespace SharpDevLib.Extensions.Encryption;

/// <summary>
/// rsa verify sign option
/// </summary>
public class RsaVerifySignOption
{
    /// <summary>
    /// instantient rsa verify sign option
    /// </summary>
    /// <param name="signature">signature to verify</param>
    /// <param name="publicKey">rsa public key,pem format,if not,use service to transform to pem format</param>
    /// <param name="hashAlgorithmName">hash algorithm name,default is sha256</param>
    public RsaVerifySignOption(byte[] signature, string publicKey, HashAlgorithmName? hashAlgorithmName = null)
    {
        Signature = signature;
        PublicKey = publicKey?.Trim() ?? string.Empty;
        HashAlgorithmName = hashAlgorithmName ?? HashAlgorithmName.SHA256;
    }

    /// <summary>
    /// signature
    /// </summary>
    public byte[] Signature { get; }
    /// <summary>
    /// rsa Public key
    /// </summary>
    public string PublicKey { get; }
    /// <summary>
    /// hash algorithm name
    /// </summary>
    public HashAlgorithmName HashAlgorithmName { get; }
    /// <summary>
    /// signature padding,default is pkcs1
    /// </summary>
    public RSASignaturePadding? SignaturePadding { get; set; }
}
