namespace SharpDevLib.Extensions.Encryption;

/// <summary>
/// rsa key type
/// </summary>
public enum RsaKeyType
{
    /// <summary>
    /// unknown type
    /// </summary>
    UnKnown,
    /// <summary>
    /// xml(RSAParameters) format
    /// </summary>
    Xml,
    /// <summary>
    /// pkcs1 format
    /// </summary>
    Pkcs1,
    /// <summary>
    /// pkcs8 format
    /// </summary>
    Pkcs8,
    /// <summary>
    /// pkcs1 or pkcs8 public
    /// </summary>
    PemPublic,
}
