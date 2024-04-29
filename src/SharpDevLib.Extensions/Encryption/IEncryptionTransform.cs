namespace SharpDevLib.Extensions.Encryption;

/// <summary>
/// encryption transform
/// </summary>
/// <typeparam name="TEncryptOption">encrypt option</typeparam>
/// <typeparam name="TDecryptOption">decrypt option</typeparam>
public interface IEncryptionTransform<TEncryptOption, TDecryptOption> where TDecryptOption : class where TEncryptOption : class
{
    /// <summary>
    /// generate a random iv
    /// </summary>
    /// <returns></returns>
    byte[] GenerateIV(byte[]? init = null);

    /// <summary>
    /// encrypt string
    /// </summary>
    /// <param name="data">string</param>
    /// <param name="option">encryption option</param>
    /// <returns>encrypted byte array</returns>
    byte[] Encrypt(string data, TEncryptOption option);
    /// <summary>
    /// encrypt file
    /// </summary>
    /// <param name="filename">file path</param>
    /// <param name="targetFileName">encrypted file path</param>
    /// <param name="option">encryption option</param>
    /// <returns>encrypted bytes</returns>
    void EncryptFile(string filename, string targetFileName, TEncryptOption option);
    /// <summary>
    /// encrypt file
    /// </summary>
    /// <param name="filename">file path</param>
    /// <param name="encryptedStream">encrypted stream</param>
    /// <param name="option">encryption option</param>
    /// <returns>encrypted bytes</returns>
    void EncryptFile(string filename, Stream encryptedStream, TEncryptOption option);
    /// <summary>
    /// decrypt string
    /// </summary>
    /// <param name="data">encrypted string</param>
    /// <param name="option">decryption option</param>
    /// <returns>decrypted byte array</returns>
    byte[] Decrypt(string data, TDecryptOption option);
    /// <summary>
    /// decrypt file
    /// </summary>
    /// <param name="filename">encrypted file path</param>
    /// <param name="decryptedFileName">decrypted file path</param>
    /// <param name="option">decryption option</param>
    /// <returns>decrypted byte array</returns>
    void DecryptFile(string filename, string decryptedFileName, TDecryptOption option);
    /// <summary>
    /// decrypt file
    /// </summary>
    /// <param name="filename">encrypted file path</param>
    /// <param name="decryptedStream">decrypted stream</param>
    /// <param name="option">decryption option</param>
    /// <returns>decrypted byte array</returns>
    void DecryptFile(string filename, Stream decryptedStream, TDecryptOption option);
}
