namespace SharpDevLib.Extensions.Encryption;

/// <summary>
/// rsa key format
/// </summary>
public enum RsaKeyFormat
{
    /// <summary>
    /// unknown format
    /// </summary>
    UnKnown,
    /// <summary>
    /// xml(RSAParameters) private format
    /// </summary>
    XmlPrivate,
    /// <summary>
    /// xml(RSAParameters) public format
    /// </summary>
    XmlPublic,
    /// <summary>
    /// pkcs1 private format
    /// </summary>
    Pkcs1Private,
    /// <summary>
    /// pkcs1 encrypted private format
    /// </summary>
    Pkcs1EncryptedPrivate,
    /// <summary>
    /// pkcs8 private format
    /// </summary>
    Pkcs8Private,
    /// <summary>
    /// encrypted pkcs8 private format
    /// </summary>
    Pkcs8EncryptedPrivate,
    /// <summary>
    /// pkcs1 or pkcs88 public format
    /// </summary>
    PemPublic,
}
