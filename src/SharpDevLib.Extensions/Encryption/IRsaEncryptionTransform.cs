namespace SharpDevLib.Extensions.Encryption;

/// <summary>
/// rsa encryption transform
/// </summary>
public interface IRsaEncryptionTransform
{
    /// <summary>
    /// encrypt string
    /// </summary>
    /// <param name="data">string</param>
    /// <param name="option">encryption option</param>
    /// <returns>encrypted byte array</returns>
    byte[] Encrypt(string data, RsaEncryptOption option);

    /// <summary>
    /// decrypt string
    /// </summary>
    /// <param name="data">encrypted string</param>
    /// <param name="option">decryption option</param>
    /// <returns>decrypted byte array</returns>
    byte[] Decrypt(string data, RsaDecryptOption option);
    
    /// <summary>
    /// rsa sign
    /// </summary>
    /// <param name="data">string to sign</param>
    /// <param name="option">rsa sign option</param>
    /// <returns>rsa signed byte array</returns>
    byte[] Sign(string data, RsaSignOption option);

    /// <summary>
    /// rsa verify sign
    /// </summary>
    /// <param name="data">string to verify</param>
    /// <param name="option">rsa verify sign option</param>
    /// <returns>data is verified</returns>
    bool VerifySign(string data, RsaVerifySignOption option);
}
