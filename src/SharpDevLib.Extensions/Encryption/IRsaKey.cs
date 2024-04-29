using System.Security.Cryptography;

namespace SharpDevLib.Extensions.Encryption;

/// <summary>
/// rsa key service
/// </summary>
public interface IRsaKey
{
    /// <summary>
    /// export password protected private key
    /// </summary>
    /// <param name="type">key type</param>
    /// <param name="size">key size,default is 2048</param>
    /// <returns>password protected private key</returns>
    string ExportPrivateKey(RsaKeyType type, int? size = 2048);

    /// <summary>
    /// export public key by password protected private key
    /// </summary>
    /// <param name="privateKey">password protected private key</param>
    /// <param name="password">key password</param>
    /// <returns>public key</returns>
    string ExportPublicKey(string privateKey, string? password = null);

    /// <summary>
    /// protect private key with password,algrithm is aes-cbc-256
    /// </summary>
    /// <param name="privateKey">private key</param>
    /// <param name="password">key password</param>
    /// <returns>password protected private key</returns>
    string AddPrivateKeyPassword(string privateKey, string password);

    /// <summary>
    /// remove password of protected private key
    /// </summary>
    /// <param name="privateKey">password protected private key</param>
    /// <param name="password">key password</param>
    /// <returns>unprotected private key</returns>
    string RemovePrivateKeyPassword(string privateKey, string password);

    /// <summary>
    /// modify password protected private key passphrase
    /// </summary>
    /// <param name="privateKey">password protected private key</param>
    /// <param name="oldPassword">old key password</param>
    /// <param name="newPassword">new key password</param>
    /// <returns>password protected private key</returns>
    string UpdatePrivateKeyPassword(string privateKey, string oldPassword, string newPassword);

    /// <summary>
    /// convert key type
    /// </summary>
    /// <param name="key">key</param>
    /// <param name="targetType">target key type</param>
    /// <returns>converted key</returns>
    string ConvertType(string key, RsaKeyType targetType);

    /// <summary>
    /// format key with '-----BEGING XXX KEY-----' AND '-----END XXX KEY-----' and wrap lines
    /// </summary>
    /// <param name="key">key</param>
    /// <param name="format">format</param>
    /// <returns>formated key</returns>
    string Format(string key, RsaKeyFormat format);

    /// <summary>
    /// remove key of '-----BEGING XXX KEY-----' AND '-----END XXX KEY-----' and wrap lines format
    /// </summary>
    /// <param name="key">key</param>
    /// <returns>unformated key</returns>
    string RemoveFormat(string key);

    /// <summary>
    /// is private/public key match
    /// </summary>
    /// <param name="privateKey">private key</param>
    /// <param name="publicKey">public key</param>
    /// <param name="password">private key password</param>
    /// <returns>is match</returns>
    bool IsKeyPairMatch(string privateKey, string publicKey, string? password = null);

    /// <summary>
    /// get key type
    /// </summary>
    /// <param name="key">key</param>
    /// <returns>key type</returns>
    RsaKeyType KeyType(string key);

    /// <summary>
    /// get key format
    /// </summary>
    /// <param name="key">key</param>
    /// <returns>key format</returns>
    RsaKeyFormat KeyFormat(string key);

    /// <summary>
    /// indicate key is a private
    /// </summary>
    /// <param name="key">key</param>
    /// <returns>is a private key</returns>
    bool IsPrivate(string key);

    /// <summary>
    /// xml to rsa parameters
    /// </summary>
    /// <param name="xmlKey">xml key</param>
    /// <returns>rsa parameters</returns>
    RSAParameters ToParameter(string xmlKey);

    /// <summary>
    /// rsa parameters to xml
    /// </summary>
    /// <param name="parameters">rsa parameters</param>
    /// <returns>xml</returns>
    string ToXml(RSAParameters parameters);

    /// <summary>
    /// indicate key has a password
    /// </summary>
    /// <param name="key">key</param>
    /// <returns>bool value</returns>
    bool IsKeyHasPassword(string key);
}
