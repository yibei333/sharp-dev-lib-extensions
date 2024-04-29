namespace SharpDevLib.Extensions.Encryption.Implemention.RsaKeyImp;

internal class RsaKeyFormatConst
{
    public const string XmlStart = "<RSAKeyValue>";
    public const string Pkcs1PrivateStart = "-----BEGIN RSA PRIVATE KEY-----";
    public const string Pkcs1PrivateEnd = "-----END RSA PRIVATE KEY-----";
    public const string Pkcs8PrivateStart = "-----BEGIN PRIVATE KEY-----";
    public const string Pkcs8EncryptedPrivateStart = "-----BEGIN ENCRYPTED PRIVATE KEY-----";
    public const string Pkcs8PrivateEnd = "-----END PRIVATE KEY-----";
    public const string Pkcs8EncryptedPrivateEnd = "-----END ENCRYPTED PRIVATE KEY-----";
    public const string PemPublicStart = "-----BEGIN PUBLIC KEY-----";
    public const string PemPublicEnd = "-----END PUBLIC KEY-----";
}
