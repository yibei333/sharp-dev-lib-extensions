using System.Security.Cryptography;

namespace SharpDevLib.Extensions.Encryption;

/// <summary>
/// rsa decrypt option
/// </summary>
public class RsaDecryptOption
{
    /// <summary>
    /// instantient rsa decrypt option
    /// </summary>
    /// <param name="privateKey">private key</param>
    public RsaDecryptOption(string privateKey)
    {
        PrivateKey = privateKey?.Trim() ?? string.Empty;
    }

    /// <summary>
    /// private key
    /// </summary>
    public string PrivateKey { get; }
    /// <summary>
    /// private key password
    /// </summary>
    public string? PrivateKeyPassword { get; set; }
    /// <summary>
    /// padding,default is pkcs1
    /// </summary>
    public RSAEncryptionPadding? Padding { get; set; }
}