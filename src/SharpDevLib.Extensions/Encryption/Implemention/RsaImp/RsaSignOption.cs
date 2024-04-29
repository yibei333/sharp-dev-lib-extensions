using System.Security.Cryptography;

namespace SharpDevLib.Extensions.Encryption;

/// <summary>
/// rsa sign option
/// </summary>
public class RsaSignOption
{
    /// <summary>
    /// instantient rsa sign option
    /// </summary>
    /// <param name="privateKey">rsa private key,pem format,if not,use service to transform to pem format</param>
    /// <param name="hashAlgorithmName">hash algorithm name,default is sha256</param>
    public RsaSignOption(string privateKey, HashAlgorithmName? hashAlgorithmName = null)
    {
        PrivateKey = privateKey?.Trim() ?? string.Empty;
        HashAlgorithmName = hashAlgorithmName ?? HashAlgorithmName.SHA256;
    }

    /// <summary>
    /// rsa private key
    /// </summary>
    public string PrivateKey { get; }
    /// <summary>
    /// rsa private key password
    /// </summary>
    public string? PrivateKeyPassword { get; set; }
    /// <summary>
    /// hash algorithm name
    /// </summary>
    public HashAlgorithmName HashAlgorithmName { get; }
    /// <summary>
    /// signature padding,default is pkcs1
    /// </summary>
    public RSASignaturePadding? SignaturePadding { get; set; }
}
