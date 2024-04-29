using System.Security.Cryptography;

namespace SharpDevLib.Extensions.Encryption;

/// <summary>
/// rsa encrypt option
/// </summary>
public class RsaEncryptOption
{
    /// <summary>
    /// instantient rsa encrypt option
    /// </summary>
    /// <param name="publicKey">public key</param>
    public RsaEncryptOption(string publicKey)
    {
        PublicKey = publicKey?.Trim() ?? string.Empty;
    }

    /// <summary>
    /// public key
    /// </summary>
    public string PublicKey { get; }
    /// <summary>
    /// padding,default is pkcs1
    /// </summary>
    public RSAEncryptionPadding? Padding { get; set; }
}
